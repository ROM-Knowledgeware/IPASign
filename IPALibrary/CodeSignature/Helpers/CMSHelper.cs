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
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using IPALibrary.MachO;
using Utilities;

namespace IPALibrary.CodeSignature
{
    public class CMSHelper
    {
        public static byte[] GenerateSignature(List<X509Certificate> certificateChain, AsymmetricKeyEntry privateKey, byte[] messageToSign)
        {
            X509Certificate signingCertificate = certificateChain[certificateChain.Count - 1];
#if MAX_CMS_COMPATIBILITY
            // Optional: This is the order that codesign uses:
            List<X509Certificate> cmsChain = new List<X509Certificate>();
            if (certificateChain.Count > 1)
            {
                cmsChain.AddRange(certificateChain.GetRange(0, certificateChain.Count - 1));
                cmsChain.Reverse();
            }
            cmsChain.Add(signingCertificate);
            certificateChain = cmsChain;
#endif
            IX509Store certificateStore = X509StoreFactory.Create("Certificate/Collection", new X509CollectionStoreParameters(certificateChain));
            
            CmsSignedDataGenerator generator = new CmsSignedDataGenerator();
#if MAX_CMS_COMPATIBILITY
            // Optional: BouncyCastle v1.8.3 has the option to use DER instead of BER to store the certificate chain
            generator.UseDerForCerts = true;
#endif
            generator.AddSigner(privateKey.Key, signingCertificate, CmsSignedDataGenerator.DigestSha256);
            generator.AddCertificates(certificateStore);
            CmsSignedData cmsSignature = generator.Generate(CmsSignedGenerator.Data, new CmsProcessableByteArray(messageToSign), false);
            return cmsSignature.GetEncoded();
        }

        public static bool ValidateSignature(byte[] messageBytes, byte[] signatureBytes, X509Certificate certificate)
        {
            CmsProcessable signedContent = new CmsProcessableByteArray(messageBytes);
            CmsSignedData signedData = new CmsSignedData(signedContent, signatureBytes);
            IX509Store certificateStore = signedData.GetCertificates("Collection");
            SignerInformationStore signerInfoStore = signedData.GetSignerInfos();
            ICollection signers = signerInfoStore.GetSigners();
            foreach (SignerInformation signer in signers)
            {
                bool isChainValid = ValidateCertificateChain(certificateStore, signer.SignerID);
                if (!isChainValid)
                {
                    return false;
                }

                bool verified = signer.Verify(certificate);
                if (!verified)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool ValidateCertificateChain(IX509Store certificateStore, SignerID signerID)
        {
            List<X509Certificate> certificateChain = GetCertificateChain(certificateStore, signerID);
            List<byte[]> certificateChainBytes = new List<byte[]>();
            foreach (X509Certificate certficate in certificateChain)
            {
                certificateChainBytes.Add(certficate.GetEncoded());
            }

            return CertificateValidationHelper.VerifyCertificateChain(certificateChainBytes);
        }

        /// <returns>The first element in the returned list will be the leaf, and the last will be the root certificate</returns>
        public static List<X509Certificate> GetCertificateChain(IX509Store certificateStore, SignerID signerID)
        {
            X509Certificate parent = null;
            List<X509Certificate> chain = new List<X509Certificate>();
            SignerID nextSignerID = signerID;
            do
            {
                ICollection matches = certificateStore.GetMatches(nextSignerID);
                foreach (X509Certificate certificate in matches)
                {
                    parent = certificate;
                    nextSignerID = new SignerID();
                    nextSignerID.Subject = certificate.IssuerDN;
                    break;
                }
                chain.Add(parent);
            }
            while (parent != null && !parent.IssuerDN.Equivalent(parent.SubjectDN));

            if (parent != null)
            {
                return chain;
            }
            else
            {
                return null;
            }
        }
    }
}
