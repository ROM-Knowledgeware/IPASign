/* Copyright (C) 2017-2018 ROM Knowledgeware. All rights reserved.
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
using System.Security.Cryptography;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using IPALibrary.MachO;
using Utilities;

namespace IPALibrary.CodeSignature
{
    public class CodeSignatureHelper
    {
        public const int SpecialHashCount = 5;

        public static readonly byte[] APPLE_ADS_OID =       { 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x63, 0x64 };
        public static readonly byte[] APPLE_EXTENSION_OID = { 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x63, 0x64, 0x06 };
        public static readonly byte[] APPLE_IOS_OID =       { 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x63, 0x64, 0x06, 0x02, 0x01 };

        public static bool ValidateExecutableHash(MachObjectFile file)
        {
            byte[] codeSignatureBytes = file.GetCodeSignatureBytes();
            if (CodeSignatureSuperBlob.IsCodeSignatureSuperBlob(codeSignatureBytes, 0))
            {
                CodeSignatureSuperBlob codeSignature = new CodeSignatureSuperBlob(codeSignatureBytes, 0);
                CodeDirectoryBlob codeDirectory = codeSignature.GetEntry(CodeSignatureEntryType.CodeDirectory) as CodeDirectoryBlob;
                byte[] signedFileData = ByteReader.ReadBytes(file.GetBytes(), 0, (int)codeDirectory.CodeLimit);
                List<byte[]> hashes = HashAlgorithmHelper.ComputeHashes(codeDirectory.HashType, codeDirectory.PageSize, signedFileData);
                if (hashes.Count != codeDirectory.CodeHashes.Count)
                {
                    return false;
                }
                for(int index = 0; index < hashes.Count; index++)
                {
                    if (!ByteUtils.AreByteArraysEqual(hashes[index], codeDirectory.CodeHashes[index]))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static bool ValidateSpecialHashes(MachObjectFile file, byte[] infoFileBytes, byte[] codeResourcesBytes)
        {
            byte[] codeSignatureBytes = file.GetCodeSignatureBytes();
            if (CodeSignatureSuperBlob.IsCodeSignatureSuperBlob(codeSignatureBytes, 0))
            {
                CodeSignatureSuperBlob codeSignature = new CodeSignatureSuperBlob(codeSignatureBytes, 0);
                CodeDirectoryBlob codeDirectory = codeSignature.GetEntry(CodeSignatureEntryType.CodeDirectory) as CodeDirectoryBlob;

                byte[] infoFileHash = HashAlgorithmHelper.ComputeHash(codeDirectory.HashType, infoFileBytes);
                if (!ByteUtils.AreByteArraysEqual(infoFileHash, codeDirectory.SpecialHashes[codeDirectory.SpecialHashes.Count - CodeDirectoryBlob.InfoFileHashOffset]))
                {
                    return false;
                }

                byte[] codeResourcesHash = HashAlgorithmHelper.ComputeHash(codeDirectory.HashType, codeResourcesBytes);
                if (!ByteUtils.AreByteArraysEqual(codeResourcesHash, codeDirectory.SpecialHashes[codeDirectory.SpecialHashes.Count - CodeDirectoryBlob.CodeResourcesFileHashOffset]))
                {
                    return false;
                }

                CodeRequirementsBlob codeRequirements = codeSignature.GetEntry(CodeSignatureEntryType.Requirements) as CodeRequirementsBlob;
                byte[] codeRequirementsBytes = codeRequirements.GetBytes();
                byte[] codeRequirementsHash = HashAlgorithmHelper.ComputeHash(codeDirectory.HashType, codeRequirementsBytes);
                if (!ByteUtils.AreByteArraysEqual(codeRequirementsHash, codeDirectory.SpecialHashes[codeDirectory.SpecialHashes.Count - CodeDirectoryBlob.RequirementsHashOffset]))
                {
                    return false;
                }

                if (codeDirectory.SpecialHashes.Count >= CodeDirectoryBlob.EntitlementsHashOffset)
                {
                    CodeSignatureGenericBlob entitlements = codeSignature.GetEntry(CodeSignatureEntryType.Entitlements) as CodeSignatureGenericBlob;
                    byte[] entitlementsBytes = entitlements.GetBytes();
                    byte[] entitlementsHash = HashAlgorithmHelper.ComputeHash(codeDirectory.HashType, entitlementsBytes);
                    if (!ByteUtils.AreByteArraysEqual(entitlementsHash, codeDirectory.SpecialHashes[codeDirectory.SpecialHashes.Count - CodeDirectoryBlob.EntitlementsHashOffset]))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static bool ValidateExecutableSignature(MachObjectFile file, X509Certificate certificate)
        {
            byte[] codeSignatureBytes = file.GetCodeSignatureBytes();
            if (CodeSignatureSuperBlob.IsCodeSignatureSuperBlob(codeSignatureBytes, 0))
            {
                CodeSignatureSuperBlob codeSignature = new CodeSignatureSuperBlob(codeSignatureBytes, 0);

                CodeDirectoryBlob codeDirectory = codeSignature.GetEntry(CodeSignatureEntryType.CodeDirectory) as CodeDirectoryBlob;
                CmsSignatureBlob cmsSignature = codeSignature.GetEntry(CodeSignatureEntryType.CmsSignature) as CmsSignatureBlob;
                if (codeDirectory == null || cmsSignature == null)
                {
                    return false;
                }

                return CMSHelper.ValidateSignature(codeDirectory.GetBytes(), cmsSignature.Data, certificate);
            }

            return false;
        }

        public static void ResignExecutable(MachObjectFile file, string bundleIdentifier, List<X509Certificate> certificateChain, AsymmetricKeyEntry privateKey, byte[] infoFileBytes, byte[] codeResourcesBytes, EntitlementsFile entitlements)
        {
            X509Certificate signingCertificate = certificateChain[certificateChain.Count - 1];
            string certificateCN = CertificateHelper.GetCertificateCommonName(signingCertificate);
            string teamID = CertificateHelper.GetCertificateOrganizationalUnit(signingCertificate);

            SegmentCommand linkEditSegment = SegmentCommandHelper.FindLinkEditSegment(file.LoadCommands);
            if (linkEditSegment == null)
            {
                throw new InvalidDataException("LinkEdit segment was not found");
            }

            if (file.LoadCommands[file.LoadCommands.Count - 1].CommandType != LoadCommandType.CodeSignature)
            {
                throw new NotImplementedException("The last LoadCommand entry is not CodeSignature");
            }

            CodeSignatureCommand command = (CodeSignatureCommand)file.LoadCommands[file.LoadCommands.Count - 1];
            int codeLength = (int)command.DataOffset;
            CodeDirectoryBlob codeDirectory = CreateCodeDirectoryBlob(codeLength, bundleIdentifier, teamID);
            CodeRequirementsBlob codeRequirements = CreateCodeRequirementsBlob(bundleIdentifier, certificateCN);
            EntitlementsBlob entitlementsBlob = CreateEntitlementsBlob(entitlements);
            CmsSignatureBlob cmsSignature = new CmsSignatureBlob();
            // We create a dummy signature to determine the length required
            cmsSignature.Data = CMSHelper.GenerateSignature(certificateChain, privateKey, codeDirectory.GetBytes());

            CodeSignatureSuperBlob codeSignature = new CodeSignatureSuperBlob();
            codeSignature.Entries.Add(CodeSignatureEntryType.CodeDirectory, codeDirectory);
            codeSignature.Entries.Add(CodeSignatureEntryType.Requirements, codeRequirements);
            codeSignature.Entries.Add(CodeSignatureEntryType.Entitlements, entitlementsBlob);
            codeSignature.Entries.Add(CodeSignatureEntryType.CmsSignature, cmsSignature);

            command.DataSize = (uint)codeSignature.Length;
            uint finalFileSize = command.DataOffset + command.DataSize;
            SegmentCommandHelper.SetEndOffset(linkEditSegment, finalFileSize);

            byte[] codeToHash = ByteReader.ReadBytes(file.GetBytes(), 0, codeLength);
            UpdateHashes(codeDirectory, codeToHash, infoFileBytes, codeRequirements, codeResourcesBytes, entitlementsBlob);
            cmsSignature.Data = CMSHelper.GenerateSignature(certificateChain, privateKey, codeDirectory.GetBytes());

            // Store updated code signature:
            byte[] codeSignatureBytes = codeSignature.GetBytes();
            Array.Resize<byte>(ref file.Data, (codeLength - file.DataOffset) + (int)command.DataSize);
            ByteWriter.WriteBytes(file.Data, (int)command.DataOffset - file.DataOffset, codeSignatureBytes);
        }

        public static CodeDirectoryBlob CreateCodeDirectoryBlob(int codeLength, string ident, string teamID)
        {
            HashType hashType = HashType.SHA256;
            int pageSize = 4096;

            CodeDirectoryBlob codeDirectory = new CodeDirectoryBlob();
            codeDirectory.CodeLimit = (uint)codeLength;
            codeDirectory.HashType = hashType;
            codeDirectory.HashSize = (byte)HashAlgorithmHelper.GetHashLength(hashType);
            codeDirectory.PageSize = pageSize;

            codeDirectory.Ident = ident;
            codeDirectory.TeamID = teamID;
            // We put empty hashes as placeholder to ensure that the blob length will not change later.
            for (int index = 0; index < SpecialHashCount; index++)
            {
                codeDirectory.SpecialHashes.Add(new byte[codeDirectory.HashSize]);
            }
            int codeHashEntries = (int)Math.Ceiling((double)codeLength / pageSize);
            for (int index = 0; index < codeHashEntries; index++)
            {
                codeDirectory.CodeHashes.Add(new byte[codeDirectory.HashSize]);
            }
            
            return codeDirectory;
        }

        public static void UpdateHashes(CodeDirectoryBlob codeDirectory, byte[] codeToHash, byte[] infoFileBytes, CodeRequirementsBlob codeRequirements, byte[] codeResourcesBytes, EntitlementsBlob entitlements)
        {
            codeDirectory.CodeHashes = HashAlgorithmHelper.ComputeHashes(codeDirectory.HashType, codeDirectory.PageSize, codeToHash);
            
            codeDirectory.SpecialHashes = new List<byte[]>();
            codeDirectory.SpecialHashes.Insert(0, (HashAlgorithmHelper.ComputeHash(codeDirectory.HashType, infoFileBytes)));
            codeDirectory.SpecialHashes.Insert(0, (HashAlgorithmHelper.ComputeHash(codeDirectory.HashType, codeRequirements.GetBytes())));
            codeDirectory.SpecialHashes.Insert(0, (HashAlgorithmHelper.ComputeHash(codeDirectory.HashType, codeResourcesBytes)));
            if (SpecialHashCount >= CodeDirectoryBlob.ApplicationSpecificHashOffset)
            {
                codeDirectory.SpecialHashes.Insert(0, new byte[HashAlgorithmHelper.GetHashLength(codeDirectory.HashType)]);
                if (SpecialHashCount >= CodeDirectoryBlob.EntitlementsHashOffset)
                {
                    codeDirectory.SpecialHashes.Insert(0, (HashAlgorithmHelper.ComputeHash(codeDirectory.HashType, entitlements.GetBytes())));
                }
            }
        }

        public static CodeRequirementsBlob CreateCodeRequirementsBlob(string ident, string certificateCN)
        {
            CodeRequirementsBlob codeRequirements = new CodeRequirementsBlob();
            CodeRequirementBlob codeRequirement = new CodeRequirementBlob();
            codeRequirements.Entries.Add(SecurityRequirementType.DesignatedRequirementType, codeRequirement);

            IdentValue identValue = new IdentValue(ident);
            AppleGenericAnchor appleGenericAnchor = new AppleGenericAnchor();
            CertificateField certificateField = new CertificateField();
            certificateField.CertificateIndex = 0;
            certificateField.FieldName = "subject.CN";
            certificateField.Match = new MatchSuffix(MatchOperationName.Equal, certificateCN);
            CertificateGeneric certificateGeneric = new CertificateGeneric();
            certificateGeneric.CertificateIndex = 1;
            certificateGeneric.OID = APPLE_IOS_OID;
            certificateGeneric.Match = new MatchSuffix(MatchOperationName.Exists);

            codeRequirement.Expression = new AndExpression(identValue, new AndExpression(appleGenericAnchor, new AndExpression(certificateField, certificateGeneric)));
            
            return codeRequirements;
        }

        public static EntitlementsBlob CreateEntitlementsBlob(EntitlementsFile entitlements)
        {
            EntitlementsBlob entitlementsBlob = new EntitlementsBlob();
#if MAX_PLIST_COMPATIBILITY
            // XCode will remove the keychain-access-groups key from embedded entitlements
            // More info: https://github.com/openbakery/gradle-xcodePlugin/issues/220
            if (entitlements.RootNode is PListNet.Nodes.DictionaryNode)
            {
                ((PListNet.Nodes.DictionaryNode)entitlements.RootNode).Remove("keychain-access-groups");
            }
#endif
            entitlementsBlob.Data = entitlements.GetBytes(PListNet.PListFormat.Xml);
            return entitlementsBlob;
        }
    }
}
