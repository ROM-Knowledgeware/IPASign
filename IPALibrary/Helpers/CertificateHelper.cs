/* Copyright (C) 2017 ROM Knowledgeware. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 * 
 * Maintainer: Tal Aloni <tal@kmrom.com>
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using Utilities;

namespace IPALibrary
{
    public class CertificateHelper
    {
        public const string X509CertificateCommonNameOID = "2.5.4.3";
        public const string X509CertificateOrganizationalUnitOID = "2.5.4.11";
        public const string X509CertificateUserOID = "0.9.2342.19200300.100.1.1";

        public static string GetCertificateTeamID(string certificateCN)
        {
            int index = certificateCN.LastIndexOf('(');
            if (index >= 0 && certificateCN[certificateCN.Length - 1] == ')')
            {
                return certificateCN.Substring(index, certificateCN.Length - index - 1);
            }
            return null;
        }

        public static X509Certificate GetCertificateAndKeyFromBytes(byte[] certificateBytes, string password, out AsymmetricKeyEntry privateKey)
        {
            privateKey = null;
            MemoryStream certificateStream = new MemoryStream(certificateBytes);
            Pkcs12Store certificateStore;
            try
            {
                certificateStore = new Pkcs12Store(certificateStream, password.ToCharArray());
            }
            catch(IOException)
            {
                return null;
            }

            foreach (string alias in certificateStore.Aliases)
            {
                AsymmetricKeyEntry key = certificateStore.GetKey(alias);
                X509CertificateEntry certificateEntry = certificateStore.GetCertificate(alias);
                X509Certificate certificate = certificateEntry.Certificate;
                if (key != null)
                {
                    privateKey = key;
                    return certificate;
                }
            }
            return null;
        }

        public static X509Certificate GetCertificatesFromBytes(byte[] certificateBytes)
        {
            return new X509CertificateParser().ReadCertificate(certificateBytes);
        }

        public static string GetCertificateCommonName(byte[] certificateBytes)
        {
            X509Certificate certificate = GetCertificatesFromBytes(certificateBytes);
            return GetCertificateCommonName(certificate);
        }

        public static string GetCertificateCommonName(X509Certificate certificate)
        {
            return GetCertificateValue(certificate, X509CertificateCommonNameOID);
        }

        public static string GetCertificateOrganizationalUnit(X509Certificate certificate)
        {
            return GetCertificateValue(certificate, X509CertificateOrganizationalUnitOID);
        }

        public static string GetCertificateUserID(X509Certificate certificate)
        {
            return GetCertificateValue(certificate, X509CertificateUserOID);
        }

        public static string GetCertificateValue(X509Certificate certificate, string identifier)
        {
            DerObjectIdentifier commonNameOID = new Org.BouncyCastle.Asn1.DerObjectIdentifier(identifier);
            IList values = certificate.SubjectDN.GetValueList(commonNameOID);
            if (values.Count > 0)
            {
                return (string)values[0];
            }
            return null;
        }

        public static X509Certificate FindBySubjectDN(List<X509Certificate> certificates, X509Name subjectDN)
        {
            foreach (X509Certificate certificate in certificates)
            {
                if (certificate.SubjectDN.Equivalent(subjectDN))
                {
                    return certificate;
                }
            }
            return null;
        }

        public static List<X509Certificate> BuildCertificateChain(X509Certificate leaf, List<X509Certificate> certificateStore)
        {
            List<X509Certificate> chain = new List<X509Certificate>();
            chain.Add(leaf);

            X509Certificate node = leaf;
            while (!node.IssuerDN.Equivalent(node.SubjectDN))
            {
                X509Certificate parent = FindBySubjectDN(certificateStore, node.IssuerDN);
                if (parent == null)
                {
                    return null;
                }
                chain.Insert(0, parent);
                node = parent;
            }
            return chain;
        }

        /// <summary>
        /// TBS: To Be Signed - the portion of the X.509 certificate that is signed with the CA's private key.
        /// </summary>
        // http://www.codeproject.com/Questions/252741/Where-is-the-signature-value-in-the-certificate
        public static byte[] ExtractTbsCertificate(byte[] certificateBytes)
        {
            if (certificateBytes[0] == 0x30 && certificateBytes[1] == 0x82)
            {
                if (certificateBytes[4] == 0x30 && certificateBytes[5] == 0x82)
                {
                    ushort length = BigEndianConverter.ToUInt16(certificateBytes, 6);
                    return ByteReader.ReadBytes(certificateBytes, 4, 4 + (int)length);
                }
            }
            throw new ArgumentException("The given certificate is not a BER-encoded ASN.1 X.509 certificate");
        }
    }
}
