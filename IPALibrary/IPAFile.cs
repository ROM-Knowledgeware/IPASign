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
using System.IO;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using IPALibrary.CodeSignature;
using IPALibrary.MachO;
using Utilities;

namespace IPALibrary
{
    public class IPAFile
    {
        public const string InfoFileName = "Info.plist";
        public const string MobileProvisionFileName = "embedded.mobileprovision";
        public const string CodeResourcesFilePath = "_CodeSignature/CodeResources";
        public const char ZipDirectorySeparator = '/';

        private MemoryStream m_stream = new MemoryStream();
        private ZipFile m_zipFile;
        private string m_appDirectoryPath;

        public IPAFile(Stream stream)
        {
            ByteUtils.CopyStream(stream, m_stream);
            m_stream.Position = 0;

            m_zipFile = new ZipFile(m_stream);
            m_appDirectoryPath = GetAppDirectoryPath();
            if (m_appDirectoryPath == null)
            {
                throw new InvalidDataException("Invalid directory structure for IPA file");
            }
        }

        public void Save(string path)
        {
            m_stream.Position = 0;
            FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
            ByteUtils.CopyStream(m_stream, fileStream);
            fileStream.Close();
        }

        private void Close()
        {
            m_zipFile.Close();
        }

        private string GetAppDirectoryPath()
        {
            List<ZipEntry> payloadEntries = new List<ZipEntry>();
            foreach (ZipEntry entry in m_zipFile)
            {
                if (entry.IsDirectory)
                {
                    if (entry.Name.StartsWith("Payload" + ZipDirectorySeparator) && entry.Name.Length > 8)
                    {
                        string subPath = entry.Name.Substring(8);
                        subPath = subPath.TrimEnd(ZipDirectorySeparator);
                        if (!subPath.Contains(ZipDirectorySeparator.ToString()))
                        {
                            payloadEntries.Add(entry);
                        }
                    }
                }
            }

            if (payloadEntries.Count != 1)
            {
                throw new InvalidDataException("Invalid directory structure for IPA file");
            }
            return payloadEntries[0].Name;
        }

        public MemoryStream GetFileStream(string path)
        {
            ZipEntry fileEntry = m_zipFile.GetEntry(m_appDirectoryPath + path);
            if (fileEntry != null && fileEntry.IsFile)
            {
                // We can't seek using the decompressed stream so we create a copy
                Stream sourceStream = m_zipFile.GetInputStream(fileEntry);
                MemoryStream fileStream = new MemoryStream();
                ByteUtils.CopyStream(sourceStream, fileStream);
                fileStream.Position = 0;
                return fileStream;
            }
            return null;
        }

        public byte[] GetFileBytes(string path)
        {
            ZipEntry fileEntry = m_zipFile.GetEntry(m_appDirectoryPath + path);
            if (fileEntry != null && fileEntry.IsFile)
            {
                Stream sourceStream = m_zipFile.GetInputStream(fileEntry);
                return ByteReader.ReadBytes(sourceStream, (int)fileEntry.Size);
            }
            return null;
        }

        public MobileProvisionFile GetMobileProvision()
        {
            byte[] fileBytes = GetFileBytes(MobileProvisionFileName);
            return new MobileProvisionFile(fileBytes);
        }

        public string GetExecutableName()
        {
            InfoFile infoFile = GetInfoFile();
            if (infoFile != null)
            {
                return infoFile.ExecutableName;
            }
            return null;
        }

        public string GetBundleIdentifier()
        {
            InfoFile infoFile = GetInfoFile();
            if (infoFile != null)
            {
                return infoFile.BundleIdentifier;
            }
            return null;
        }

        public byte[] GetInfoFileBytes()
        {
            return GetFileBytes(InfoFileName);
        }

        public MemoryStream GetInfoFileStream()
        {
            return GetFileStream(InfoFileName);
        }

        public InfoFile GetInfoFile()
        {
            byte[] infoBytes = GetInfoFileBytes();
            return new InfoFile(infoBytes);
        }

        public byte[] GetExecutableBytes()
        {
            string executableName = GetExecutableName();
            return GetFileBytes(executableName);
        }

        public byte[] GetCodeResourcesBytes()
        {
            return GetFileBytes(CodeResourcesFilePath);
        }

        public CodeResourcesFile GetCodeResourcesFile()
        {
            byte[] codeResourcesBytes = GetCodeResourcesBytes();
            return new CodeResourcesFile(codeResourcesBytes);
        }

        public bool ValidateExecutableSignature(X509Certificate certificate)
        {
            byte[] buffer = GetExecutableBytes();
            byte[] infoFileBytes = GetInfoFileBytes();
            byte[] codeResourcesBytes = GetFileBytes(CodeResourcesFilePath);
            List<MachObjectFile> files = MachObjectHelper.ReadMachObjects(buffer);
            foreach(MachObjectFile file in files)
            {
                if (!CodeSignatureHelper.ValidateExecutableHash(file))
                {
                    return false;
                }

                if (!CodeSignatureHelper.ValidateSpecialHashes(file, infoFileBytes, codeResourcesBytes))
                {
                    return false;
                }

                if (!CodeSignatureHelper.ValidateExecutableSignature(file, certificate))
                {
                    return false;
                }
            }

            return true;
        }

        public void ReplaceMobileProvision(byte[] mobileProvisionBytes)
        {
            m_zipFile.BeginUpdate();
            MemoryStreamDataSource mobileProvisionDataSource = new MemoryStreamDataSource(mobileProvisionBytes);
            m_zipFile.Add(mobileProvisionDataSource, m_appDirectoryPath + MobileProvisionFileName);
            
            CodeResourcesFile codeResources = GetCodeResourcesFile();
            codeResources.UpdateFileHash(MobileProvisionFileName, mobileProvisionBytes);

            MobileProvisionFile mobileProvision = new MobileProvisionFile(mobileProvisionBytes);
            string provisionedBundleIdentifier = mobileProvision.PList.Entitlements.BundleIdentifier;
            if (provisionedBundleIdentifier != GetBundleIdentifier())
            {
                // We must update the info.plist's CFBundleIdentifier to match the one from the mobile provision
                InfoFile infoFile = GetInfoFile();
                infoFile.BundleIdentifier = provisionedBundleIdentifier;
                byte[] infoBytes = infoFile.GetBytes();
                MemoryStreamDataSource infoDataSource = new MemoryStreamDataSource(infoBytes);
                m_zipFile.Add(infoDataSource, m_appDirectoryPath + InfoFileName);

                codeResources.UpdateFileHash(InfoFileName, infoBytes);
            }

            byte[] codeResourcesBytes = codeResources.GetBytes();
            MemoryStreamDataSource codeResourcesDataSource = new MemoryStreamDataSource(codeResourcesBytes);
            m_zipFile.Add(codeResourcesDataSource, m_appDirectoryPath + CodeResourcesFilePath);

            m_zipFile.CommitUpdate();
        }

        private void ReplaceExecutable(byte[] executableBytes)
        {
            m_zipFile.BeginUpdate();
            MemoryStreamDataSource executableDataSource = new MemoryStreamDataSource(executableBytes);
            string executableName = GetExecutableName();
            m_zipFile.Add(executableDataSource, m_appDirectoryPath + executableName);
            m_zipFile.CommitUpdate();
        }

        public void ResignIPA(List<X509Certificate> certificateChain, AsymmetricKeyEntry privateKey)
        {
            MobileProvisionFile mobileProvision = GetMobileProvision();
            byte[] buffer = GetExecutableBytes();
            string bundleIdentifier = GetBundleIdentifier();
            byte[] infoFileBytes = GetInfoFileBytes();
            byte[] codeResourcesBytes = GetCodeResourcesBytes();
            List<MachObjectFile> files = MachObjectHelper.ReadMachObjects(buffer);
            foreach (MachObjectFile file in files)
            {
                CodeSignatureHelper.ResignExecutable(file, bundleIdentifier, certificateChain, privateKey, infoFileBytes, codeResourcesBytes, mobileProvision.PList.Entitlements);
            }
            byte[] executableBytes = MachObjectHelper.PackMachObjects(files);

            ReplaceExecutable(executableBytes);
        }
    }
}
