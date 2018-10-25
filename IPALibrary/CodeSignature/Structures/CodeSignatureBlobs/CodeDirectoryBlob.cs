/* Copyright (C) 2017-2018 ROM Knowledgeware. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 * 
 * Maintainer: Tal Aloni <tal@kmrom.com>
 */
using System;
using System.Collections.Generic;
using Utilities;

namespace IPALibrary.CodeSignature
{
    /// <remarks>
    /// Reference:
    /// codedirectory.h (libsecurity_codesigning: https://git.saurik.com/apple/security.git )
    /// </remarks>
    public class CodeDirectoryBlob : CodeSignatureBlob
    {
        public const uint Signature = 0xfade0c02; // CSMAGIC_CODEDIRECTORY
        public const uint ScatterMinimumVersion = 0x20100;
        public const uint TeamIDMinimumVersion = 0x20200;
        public const uint CodeLimit64MinimumVersion = 0x20300;
        public const uint ExecSegMinimumVersion = 0x20400;

        public const int FixedLengthV20001 = 44;
        public const int FixedLengthV20100 = 48;
        public const int FixedLengthV20200 = 52;

        public const int InfoFileHashOffset = 1;
        public const int RequirementsHashOffset = 2;
        public const int CodeResourcesFileHashOffset = 3;
        public const int ApplicationSpecificHashOffset = 4;
        public const int EntitlementsHashOffset = 5;

        // uint Magic;
        // uint Length;
        public uint Version;
        public CodeDirectoryFlags Flags;
        // uint HashOffset;
        // uint IdentOffset;
        // uint NumberOfSpecialSlots;    // nSpecialSlots
        // uint NumberOfCodeSlots;       // nCodeSlots
        public uint CodeLimit;
        public byte HashSize;
        public HashType HashType;
        public byte Platform;
        public byte PageSizeLog2;
        public uint Spare2;
        // uint ScatterOffset;        // Version 0x20100
        // uint TeamIDOffset;         // Version 0x20200
        public uint Spare3;           // Version 0x20300
        public ulong CodeLimit64;     // Version 0x20300
        public ulong ExecSegBase;     // Version 0x20400
        public ulong ExecSegLimit;    // Version 0x20400
        public ulong ExecSegFlags;    // Version 0x20400

        public string Ident;
        public string TeamID;         // Version 0x20200
        public List<byte[]> SpecialHashes = new List<byte[]>();
        public List<byte[]> CodeHashes = new List<byte[]>();

        public CodeDirectoryBlob()
        {
            Version = 0x00020200;
        }

        public CodeDirectoryBlob(byte[] buffer, int offset) : base(buffer, offset)
        {
            uint length = BigEndianConverter.ToUInt32(buffer, offset + 4);
            Version = BigEndianConverter.ToUInt32(buffer, offset + 8);
            Flags = (CodeDirectoryFlags)BigEndianConverter.ToUInt32(buffer, offset + 12);
            uint hashOffset = BigEndianConverter.ToUInt32(buffer, offset + 16);
            uint identOffset = BigEndianConverter.ToUInt32(buffer, offset + 20);
            uint numberOfSpecialSlots = BigEndianConverter.ToUInt32(buffer, offset + 24);
            uint numberOfCodeSlots = BigEndianConverter.ToUInt32(buffer, offset + 28);
            CodeLimit = BigEndianConverter.ToUInt32(buffer, offset + 32);
            HashSize = ByteReader.ReadByte(buffer, offset + 36);
            HashType = (HashType)ByteReader.ReadByte(buffer, offset + 37);
            Platform = ByteReader.ReadByte(buffer, offset + 38);
            PageSizeLog2 = ByteReader.ReadByte(buffer, offset + 39);
            Spare2 = BigEndianConverter.ToUInt32(buffer, offset + 40);
            uint scatterOffset = 0;
            uint teamIDOffset = 0;
            if (Version >= ScatterMinimumVersion)
            {
                scatterOffset = BigEndianConverter.ToUInt32(buffer, offset + 44);
                if (Version >= TeamIDMinimumVersion)
                {
                    teamIDOffset = BigEndianConverter.ToUInt32(buffer, offset + 48);
                    if (Version >= CodeLimit64MinimumVersion)
                    {
                        Spare3 = BigEndianConverter.ToUInt32(buffer, offset + 52);
                        CodeLimit64 = BigEndianConverter.ToUInt64(buffer, offset + 56);
                        if (Version >= ExecSegMinimumVersion)
                        {
                            ExecSegBase = BigEndianConverter.ToUInt64(buffer, offset + 64);
                            ExecSegLimit = BigEndianConverter.ToUInt64(buffer, offset + 72);
                            ExecSegFlags = BigEndianConverter.ToUInt64(buffer, offset + 80);
                        }
                    }
                }
            }

            if (identOffset != 0)
            {
                Ident = ByteReader.ReadNullTerminatedAnsiString(buffer, offset + (int)identOffset);
            }

            if (teamIDOffset != 0)
            {
                TeamID = ByteReader.ReadNullTerminatedAnsiString(buffer, offset + (int)teamIDOffset);
            }

            int specialHashesOffset = (int)hashOffset - (int)numberOfSpecialSlots * (int)HashSize;
            for (int index = 0; index < numberOfSpecialSlots; index++)
            {
                byte[] hash = ByteReader.ReadBytes(buffer, offset + specialHashesOffset + index * (int)HashSize, (int)HashSize);
                SpecialHashes.Add(hash);
            }

            for (int index = 0; index < numberOfCodeSlots; index++)
            {
                byte[] hash = ByteReader.ReadBytes(buffer, offset + (int)hashOffset + index * (int)HashSize, (int)HashSize);
                CodeHashes.Add(hash);
            }
        }

        public override void WriteBytes(byte[] buffer, int offset)
        {
            int fixedLength = FixedLengthV20001;
            if (Version >= ScatterMinimumVersion)
            {
                fixedLength += 4;
                if (Version >= TeamIDMinimumVersion)
                {
                    fixedLength += 4;
                    if (Version >= CodeLimit64)
                    {
                        fixedLength += 12;
                        if (Version >= ExecSegMinimumVersion)
                        {
                            fixedLength += 24;
                        }
                    }
                }
            }

            int nextOffset = fixedLength;;
            int identOffset = 0;
            if (Ident != null && Ident.Length > 0)
            {
                identOffset = nextOffset;
                nextOffset += Ident.Length + 1;
            }

            int teamIDOffset = 0;
            if (Version >= TeamIDMinimumVersion && TeamID != null && TeamID.Length > 0)
            {
                teamIDOffset = nextOffset;
                nextOffset += TeamID.Length + 1;
            }

            int specialHashesOffset = nextOffset;
            int hashOffset = specialHashesOffset + SpecialHashes.Count * HashSize;
            
            BigEndianWriter.WriteUInt32(buffer, offset + 0, Signature);
            BigEndianWriter.WriteUInt32(buffer, offset + 4, (uint)Length);
            BigEndianWriter.WriteUInt32(buffer, offset + 8, Version);
            BigEndianWriter.WriteUInt32(buffer, offset + 12, (uint)Flags);
            BigEndianWriter.WriteUInt32(buffer, offset + 16, (uint)hashOffset);
            BigEndianWriter.WriteUInt32(buffer, offset + 20, (uint)identOffset);
            BigEndianWriter.WriteUInt32(buffer, offset + 24, (uint)SpecialHashes.Count);
            BigEndianWriter.WriteUInt32(buffer, offset + 28, (uint)CodeHashes.Count);
            BigEndianWriter.WriteUInt32(buffer, offset + 32, CodeLimit);
            ByteWriter.WriteByte(buffer, offset + 36, HashSize);
            ByteWriter.WriteByte(buffer, offset + 37, (byte)HashType);
            ByteWriter.WriteByte(buffer, offset + 38, Platform);
            ByteWriter.WriteByte(buffer, offset + 39, PageSizeLog2);
            BigEndianWriter.WriteUInt32(buffer, offset + 40, Spare2);
            if (Version >= ScatterMinimumVersion)
            {
                BigEndianWriter.WriteUInt32(buffer, offset + 44, 0);
                if (Version >= TeamIDMinimumVersion)
                {
                    BigEndianWriter.WriteUInt32(buffer, offset + 48, (uint)teamIDOffset);
                    if (Version >= CodeLimit64MinimumVersion)
                    {
                        BigEndianWriter.WriteUInt32(buffer, offset + 52, Spare3);
                        BigEndianWriter.WriteUInt64(buffer, offset + 56, CodeLimit64);
                        if (Version >= ExecSegMinimumVersion)
                        {
                            BigEndianWriter.WriteUInt64(buffer, offset + 64, ExecSegBase);
                            BigEndianWriter.WriteUInt64(buffer, offset + 72, ExecSegLimit);
                            BigEndianWriter.WriteUInt64(buffer, offset + 80, ExecSegFlags);
                        }
                    }
                }
            }

            if (Ident != null && Ident.Length > 0)
            {
                ByteWriter.WriteNullTerminatedAnsiString(buffer, offset + (int)identOffset, Ident);
            }
            if (Version >= TeamIDMinimumVersion && TeamID != null && TeamID.Length > 0)
            {
                ByteWriter.WriteNullTerminatedAnsiString(buffer, offset + (int)teamIDOffset, TeamID);
            }

            for (int index = 0; index < SpecialHashes.Count; index++)
            {
                if (SpecialHashes[index].Length != HashSize)
                {
                    throw new ArgumentException("Hash length does not match declared HashSize");
                }
                ByteWriter.WriteBytes(buffer, offset + specialHashesOffset + index * (int)HashSize, SpecialHashes[index]);
            }

            for (int index = 0; index < CodeHashes.Count; index++)
            {
                if (CodeHashes[index].Length != HashSize)
                {
                    throw new ArgumentException("Hash length does not match declared HashSize");
                }
                ByteWriter.WriteBytes(buffer, offset + hashOffset + index * (int)HashSize, CodeHashes[index]);
            }
        }

        public override int Length
        {
            get
            {
                int length = FixedLengthV20001;
                if (Version >= ScatterMinimumVersion)
                {
                    length += 4;
                    if (Version >= TeamIDMinimumVersion)
                    {
                        length += 4;
                        if (Version >= CodeLimit64)
                        {
                            length += 12;
                            if (Version >= ExecSegMinimumVersion)
                            {
                                length += 24;
                            }
                        }
                    }
                }

                if (Ident != null && Ident.Length > 0)
                {
                    length += Ident.Length + 1;
                }

                if (Version >= TeamIDMinimumVersion && TeamID != null && TeamID.Length > 0)
                {
                    length += TeamID.Length + 1;
                }
                length += SpecialHashes.Count * HashSize;
                length += CodeHashes.Count * HashSize;
                return length;
            }
        }

        public int PageSize
        {
            get
            {
                return 1 << PageSizeLog2;
            }
            set
            {
                PageSizeLog2 = (byte)Math.Log(value, 2);
            }
        }

        public static bool IsCodeDirectory(byte[] buffer, int offset)
        {
            uint magic = BigEndianConverter.ToUInt32(buffer, offset + 0);
            return (magic == Signature);
        }
    }
}
