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
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Utilities;

namespace IPALibrary.CodeSignature
{
    public class CertificateValidationHelper
    {
        public static byte[] SHA_160_PKCS_ID = new byte[] { 0x30, 0x21, 0x30, 0x09, 0x06, 0x05, 0x2B, 0x0E, 0x03, 0x02, 0x1A, 0x05, 0x00, 0x04, 0x14 };
        public static byte[] SHA_256_PKCS_ID = new byte[] { 0x30, 0x31, 0x30, 0x0D, 0x06, 0x09, 0x60, 0x86, 0x48, 0x01, 0x65, 0x03, 0x04, 0x02, 0x01, 0x05, 0x00, 0x04, 0x20 };

        public static int GetRootCertificateIndex(List<byte[]> certificates)
        {
            int index;
            for (index = 0; index < certificates.Count; index++)
            {
                byte[] certificateBytes = certificates[index];
                X509Certificate2 certificate = new X509Certificate2(certificateBytes);
                if (certificate.Issuer == certificate.Subject)
                {
                    return index;
                }
            }
            return -1;
        }

        public static bool VerifyCertificateChain(List<byte[]> certificates)
        {
            int rootCertificateIndex = GetRootCertificateIndex(certificates);
            if (rootCertificateIndex == -1)
            {
                return false;
            }

            for (int index = 0; index < rootCertificateIndex; index++)
            {
                byte[] certificateBytes = certificates[index];
                X509Certificate2 certificate = new X509Certificate2(certificateBytes);
                byte[] issuingCertificateBytes = certificates[index + 1];
                bool isValid = ValidateCertificate(issuingCertificateBytes, certificateBytes);
                if (!isValid)
                {
                    return false;
                }
            }

            return ValidateCertificate(certificates[rootCertificateIndex], certificates[rootCertificateIndex]);
        }

        public static bool ValidateCertificate(byte[] issuingCertificate, byte[] certificateToValidate)
        {
            RSAParameters rsaParameters = GetRSAParameters(issuingCertificate);
            byte[] certificateSignature = ByteReader.ReadBytes(certificateToValidate, certificateToValidate.Length - 256, 256);
            byte[] decodedSignature = RSAHelper.DecryptSignature(certificateSignature, rsaParameters);
            byte[] tbsCertificate = CertificateHelper.ExtractTbsCertificate(certificateToValidate);
            if (StartsWith(decodedSignature, SHA_256_PKCS_ID))
            {
                byte[] expectedHash = ByteReader.ReadBytes(decodedSignature, SHA_256_PKCS_ID.Length, 32);
                SHA256Managed sha256 = new SHA256Managed();
                byte[] hash = sha256.ComputeHash(tbsCertificate);
                return ByteUtils.AreByteArraysEqual(hash, expectedHash);

            }
            else if (StartsWith(decodedSignature, SHA_160_PKCS_ID))
            {
                byte[] expectedHash = ByteReader.ReadBytes(decodedSignature, SHA_160_PKCS_ID.Length, 20);
                SHA1Managed sha1 = new SHA1Managed();
                byte[] hash = sha1.ComputeHash(tbsCertificate);
                return ByteUtils.AreByteArraysEqual(hash, expectedHash);
            }
            else
            {
                throw new NotImplementedException("Unsupported Signature PKCS ID");
            }
        }

        public static bool StartsWith(byte[] array1, byte[] array2)
        {
            if (array1.Length >= array2.Length)
            {
                byte[] start = ByteReader.ReadBytes(array1, 0, array2.Length);
                return ByteUtils.AreByteArraysEqual(start, array2);
            }
            return false;
        }

        public static RSAParameters GetRSAParameters(byte[] certificateBytes)
        {
            X509Certificate2 x509certificate = new X509Certificate2(certificateBytes);
            if (x509certificate.HasPrivateKey)
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)x509certificate.PrivateKey;
                return rsa.ExportParameters(true);
            }
            else
            {
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)x509certificate.PublicKey.Key;
                return rsa.ExportParameters(false);
            }
        }
    }
}
