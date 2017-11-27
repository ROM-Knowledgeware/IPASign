/* Copyright (C) 2017 ROM Knowledgeware. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 * 
 * Maintainer: Tal Aloni <tal@kmrom.com>
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using IPALibrary;

namespace IPASign
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSelectInputIPA_Click(object sender, EventArgs e)
        {
            DialogResult result = openIPADialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtInputIPA.Text = openIPADialog.FileName;
            }
        }

        private void btnSelectMobileProvision_Click(object sender, EventArgs e)
        {
            DialogResult result = openMobileProvisionDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtMobileProvision.Text = openMobileProvisionDialog.FileName;
            }
        }

        private void btnSelectSigningCertificate_Click(object sender, EventArgs e)
        {
            DialogResult result = openCertificateDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                txtSigningCertificate.Text = openCertificateDialog.FileName;
            }
        }

        private void btnTestIPA_Click(object sender, EventArgs e)
        {
            string inputIPAPath = txtInputIPA.Text;
            string signingCertificatePath = txtSigningCertificate.Text;
            string certificatePassword = txtCertificatePassword.Text;
            FileStream ipaStream;
            byte[] signingCertificateBytes;

            try
            {
                ipaStream = new FileStream(inputIPAPath, FileMode.Open, FileAccess.Read);
            }
            catch (IOException)
            {
                MessageBox.Show("Failed to read Input IPA file", "Error");
                return;
            }

            try
            {
                signingCertificateBytes = File.ReadAllBytes(signingCertificatePath);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Failed to read signing certificate file", "Error");
                return;
            }
            catch (IOException)
            {
                MessageBox.Show("Failed to read signing certificate file", "Error");
                return;
            }

            IPAFile ipaFile = new IPAFile(ipaStream);
            ValidateIPA(ipaFile, signingCertificateBytes, certificatePassword);
        }

        private void ValidateIPA(IPAFile ipaFile, byte[] signingCertificateBytes, string certificatePassword)
        {
            AsymmetricKeyEntry privateKey;
            X509Certificate signingCertificate = CertificateHelper.GetCertificateAndKeyFromBytes(signingCertificateBytes, certificatePassword, out privateKey);
            if (signingCertificate == null)
            {
                MessageBox.Show("Failed to parse the given signing certificate", "Error");
                return;
            }

            bool isValid;
            try
            {
                isValid = ipaFile.ValidateExecutableSignature(signingCertificate);
            }
            catch (Org.BouncyCastle.Security.Certificates.CertificateExpiredException)
            {
                MessageBox.Show("Certificate is outdated", "Error");
                return;
            }
            catch (Org.BouncyCastle.Security.Certificates.CertificateNotYetValidException)
            {
                MessageBox.Show("Certificate is outdated", "Error");
                return;
            }

            if (isValid)
            {
                MessageBox.Show("Signature is valid", "Success");
            }
            else
            {
                MessageBox.Show("Signature is invalid", "Error");
            }
        }

        private void btnSaveIPA_Click(object sender, EventArgs e)
        {
            string inputIPAPath = txtInputIPA.Text;
            string mobileProvisionPath = txtMobileProvision.Text;
            string signingCertificatePath = txtSigningCertificate.Text;
            string certificatePassword = txtCertificatePassword.Text;

            FileStream ipaStream;
            byte[] mobileProvisionBytes = null;
            byte[] signingCertificateBytes;
            try
            {
                ipaStream = new FileStream(inputIPAPath, FileMode.Open, FileAccess.Read);
            }
            catch (IOException)
            {
                MessageBox.Show("Failed to read Input IPA file", "Error");
                return;
            }

            if (mobileProvisionPath != String.Empty)
            {
                try
                {
                    mobileProvisionBytes = File.ReadAllBytes(mobileProvisionPath);
                }
                catch (IOException)
                {
                    MessageBox.Show("Failed to read mobile provision file", "Error");
                    return;
                }
            }

            try
            {
                signingCertificateBytes = File.ReadAllBytes(signingCertificatePath);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Failed to read signing certificate file", "Error");
                return;
            }
            catch (IOException)
            {
                MessageBox.Show("Failed to read signing certificate file", "Error");
                return;
            }

            DialogResult result = saveIPADialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string outputIPAPath = saveIPADialog.FileName;
                IPAFile ipaFile = new IPAFile(ipaStream);
                ResignIPA(ipaFile, mobileProvisionBytes, signingCertificateBytes, certificatePassword, outputIPAPath);
            }
        }

        private void ResignIPA(IPAFile ipaFile, byte[] mobileProvisionBytes, byte[] signingCertificateBytes, string certificatePassword, string outputIPAPath)
        {
            // Validate that the mobileprovision match the given certificate
            MobileProvisionFile mobileProvision;
            if (mobileProvisionBytes == null)
            {
                mobileProvision = ipaFile.GetMobileProvision();
            }
            else
            {
                mobileProvision = new MobileProvisionFile(mobileProvisionBytes);
            }

            if (mobileProvision.PList.DeveloperCertificates.Count == 0)
            {
                MessageBox.Show("Mobile Provision does not contain developer certificate information", "Error");
                return;
            }

            X509Certificate provisionedCertificate = CertificateHelper.GetCertificatesFromBytes(mobileProvision.PList.DeveloperCertificates[0]);
            AsymmetricKeyEntry privateKey;
            X509Certificate signingCertificate = CertificateHelper.GetCertificateAndKeyFromBytes(signingCertificateBytes, certificatePassword, out privateKey);
            if (signingCertificate == null)
            {
                MessageBox.Show("Failed to parse the given signing certificate", "Error");
                return;
            }

            if (!provisionedCertificate.Equals(signingCertificate))
            {
                MessageBox.Show("The signing certificate given does not match the one specified in the Mobile Provision file", "Error");
                return;
            }

            List<X509Certificate> certificateStore;
            try
            {
                certificateStore = ReadCertificatesDirectory();
            }
            catch
            {
                MessageBox.Show("Failed to read certificate directory", "Error");
                return;
            }
            
            List<X509Certificate> certificateChain = CertificateHelper.BuildCertificateChain(signingCertificate, certificateStore);

            if (mobileProvisionBytes != null)
            {
                ipaFile.ReplaceMobileProvision(mobileProvisionBytes);
            }

            ipaFile.ResignIPA(certificateChain, privateKey);
            try
            {
                ipaFile.Save(outputIPAPath);
            }
            catch (IOException ex)
            {
                MessageBox.Show("Failed to save output IPA: " + ex.Message, "Error");
                return;
            }

            MessageBox.Show("Done!");
        }

        private static string GetCertificatesPath()
        {
            string assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string assemblyDirectory = Path.GetDirectoryName(assemblyPath);
            if (!assemblyDirectory.EndsWith(@"\"))
            {
                assemblyDirectory += @"\";
            }
            return assemblyDirectory + @"Certificates\";
        }

        private static List<X509Certificate> ReadCertificatesDirectory()
        {
            List<X509Certificate> certificates = new List<X509Certificate>();
            string certificatesPath = GetCertificatesPath();
            string[] files = Directory.GetFiles(certificatesPath, "*.cer");
            foreach (string filePath in files)
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                X509Certificate certificate = CertificateHelper.GetCertificatesFromBytes(fileBytes);
                certificates.Add(certificate);
            }
            return certificates;
        }
    }
}