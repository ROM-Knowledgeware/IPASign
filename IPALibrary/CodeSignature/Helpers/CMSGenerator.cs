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
using Org.BouncyCastle;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using IPALibrary.MachO;
using Utilities;

namespace IPALibrary.CodeSignature
{
    public class CMSGenerator
    {
        public static CmsSignedData Generate(IX509Store certificateStore, X509Certificate signingCertificate, AsymmetricKeyEntry privateKey, string digestOID, CmsProcessable content)
        {
            string encOID = CmsSignedGenerator.EncryptionRsa;
            IssuerAndSerialNumber issuerAndSerialNumber = new IssuerAndSerialNumber(signingCertificate.IssuerDN, signingCertificate.SerialNumber);
            SignerIdentifier signerIdentifier = new SignerIdentifier(issuerAndSerialNumber);

            AlgorithmIdentifier digestAlgorithmID = new AlgorithmIdentifier(new DerObjectIdentifier(digestOID));
            AlgorithmIdentifier encryptionAlgorithmID = new AlgorithmIdentifier(new DerObjectIdentifier(encOID));
            
            SignerInfo signerInfo = new SignerInfo(signerIdentifier, digestAlgorithmID, (Asn1Set)null, encryptionAlgorithmID, null, null);
            return Generate(certificateStore, signerInfo, content);
        }

        public static SignerInfo ToSignerInfo(DerObjectIdentifier contentType, CmsProcessable content, SecureRandom random, AlgorithmIdentifier digestAlgorithmID)
        {
            AlgorithmIdentifier digAlgId = digestAlgorithmID;
            string digestName = Helper.GetDigestAlgName(digestOID);

            string signatureName = digestName + "with" + Helper.GetEncryptionAlgName(encOID);

            byte[] hash;
            if (outer._digests.Contains(digestOID))
            {
                hash = (byte[])outer._digests[digestOID];
            }
            else
            {
                IDigest dig = Helper.GetDigestInstance(digestName);
                if (content != null)
                {
                    content.Write(new DigOutputStream(dig));
                }
                hash = DigestUtilities.DoFinal(dig);
                outer._digests.Add(digestOID, hash.Clone());
            }

            IStreamCalculator calculator = sigCalc.CreateCalculator();

#if NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || PORTABLE
				Stream sigStr = calculator.Stream;
#else
            Stream sigStr = new BufferedStream(calculator.Stream);
#endif

            Asn1Set signedAttr = null;
            if (sAttr != null)
            {
                IDictionary parameters = outer.GetBaseParameters(contentType, digAlgId, hash);

                //					Asn1.Cms.AttributeTable signed = sAttr.GetAttributes(Collections.unmodifiableMap(parameters));
                Asn1.Cms.AttributeTable signed = sAttr.GetAttributes(parameters);

                if (contentType == null) //counter signature
                {
                    if (signed != null && signed[CmsAttributes.ContentType] != null)
                    {
                        IDictionary tmpSigned = signed.ToDictionary();
                        tmpSigned.Remove(CmsAttributes.ContentType);
                        signed = new Asn1.Cms.AttributeTable(tmpSigned);
                    }
                }

                // TODO Validate proposed signed attributes

                signedAttr = outer.GetAttributeSet(signed);

                // sig must be composed from the DER encoding.
                new DerOutputStream(sigStr).WriteObject(signedAttr);
            }
            else if (content != null)
            {
                // TODO Use raw signature of the hash value instead
                content.Write(sigStr);
            }

            Platform.Dispose(sigStr);
            byte[] sigBytes = ((IBlockResult)calculator.GetResult()).Collect();

            // TODO[RSAPSS] Need the ability to specify non-default parameters
            Asn1Encodable sigX509Parameters = SignerUtilities.GetDefaultX509Parameters(signatureName);
            AlgorithmIdentifier encAlgId = Helper.GetEncAlgorithmIdentifier(
                new DerObjectIdentifier(encOID), sigX509Parameters);

            return new SignerInfo(signerIdentifier, digAlgId,
                signedAttr, encAlgId, new DerOctetString(sigBytes), unsignedAttr);
        }

        public static CmsSignedData Generate(IX509Store certificateStore, SignerInfo signerInfo, CmsProcessable content)
        {
            Asn1EncodableVector digestAlgs = new Asn1EncodableVector();
            digestAlgs.Add(signerInfo.DigestAlgorithm);

            DerObjectIdentifier contentTypeOid = new DerObjectIdentifier(CmsObjectIdentifiers.Data.Id);

            Asn1EncodableVector signerInfos = new Asn1EncodableVector();
            signerInfos.Add(signerInfo);

            Asn1Set certificates = CreateDerSetFromList(GetCertificatesFromStore(certificateStore));
            Asn1Set certrevlist = null;

            ContentInfo encInfo = new ContentInfo(contentTypeOid, null);

            SignedData sd = new SignedData(
                new DerSet(digestAlgs),
                encInfo,
                certificates,
                certrevlist,
                new DerSet(signerInfos));

            ContentInfo contentInfo = new ContentInfo(CmsObjectIdentifiers.SignedData, sd);

            return new CmsSignedData(content, contentInfo);
        }

        public static IList GetCertificatesFromStore(
            IX509Store certStore)
        {
            try
            {
                IList certs = new ArrayList();

                if (certStore != null)
                {
                    foreach (X509Certificate c in certStore.GetMatches(null))
                    {
                        certs.Add(
                            X509CertificateStructure.GetInstance(
                                Asn1Object.FromByteArray(c.GetEncoded())));
                    }
                }

                return certs;
            }
            catch (CertificateEncodingException e)
            {
                throw new CmsException("error encoding certs", e);
            }
            catch (Exception e)
            {
                throw new CmsException("error processing certs", e);
            }
        }

        public static Asn1Set CreateDerSetFromList(
            IList derObjects)
        {
            Asn1EncodableVector v = new Asn1EncodableVector();

            foreach (Asn1Encodable ae in derObjects)
            {
                v.Add(ae);
            }

            return new DerSet(v);
        }
    }
}
