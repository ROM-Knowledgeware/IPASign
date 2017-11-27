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
using Utilities;

namespace IPALibrary.MachO
{
    public class MachHeader
    {
        public const int Length32Bit = 28;
        public const int Length64Bit = 32;

        public const uint MachO32BitLittleEndianSignature = 0xcefaedfe;
        public const uint MachO64BitLittleEndianSignature = 0xcffaedfe;
        public const uint MachO32BitBigEndianSignature = 0xfeedface;
        public const uint MachO64BitBigEndianSignature = 0xfeedfacf;
        public const uint CpuArchitecture64BitFlag = 0x01000000; // CPU_ARCH_ABI64

        private bool m_is64BitHeader;
        // uint Magic;
        public CpuType CpuType;
        public uint CpuSubType;
        public FileType FileType;
        public uint NumberOfLoadCommands; // ncmds
        public uint SizeOfLoadCommands; // sizeofcmds
        public MachHeaderFlags Flags;
        public uint Reserved; // 64-Bit only

        public MachHeader(bool is64Bit)
        {
            m_is64BitHeader = is64Bit;
        }

        public MachHeader(byte[] buffer, int offset)
        {
            uint magic = BigEndianConverter.ToUInt32(buffer, offset);
            m_is64BitHeader = (magic == MachO64BitLittleEndianSignature);
            CpuType = (CpuType)LittleEndianConverter.ToUInt32(buffer, offset + 4);
            CpuSubType = LittleEndianConverter.ToUInt32(buffer, offset + 8);
            FileType = (FileType)LittleEndianConverter.ToUInt32(buffer, offset + 12);
            NumberOfLoadCommands = LittleEndianConverter.ToUInt32(buffer, offset + 16);
            SizeOfLoadCommands = LittleEndianConverter.ToUInt32(buffer, offset + 20);
            Flags = (MachHeaderFlags)LittleEndianConverter.ToUInt32(buffer, offset + 24);
            if (m_is64BitHeader)
            {
                Reserved = LittleEndianConverter.ToUInt32(buffer, offset + 28);
            }
        }

        public void WriteBytes(byte[] buffer, int offset)
        {
            if (m_is64BitHeader)
            {
                BigEndianWriter.WriteUInt32(buffer, offset + 0, MachO64BitLittleEndianSignature);
            }
            else
            {
                BigEndianWriter.WriteUInt32(buffer, offset + 0, MachO32BitLittleEndianSignature);
            }
            LittleEndianWriter.WriteUInt32(buffer, offset + 4, (uint)CpuType);
            LittleEndianWriter.WriteUInt32(buffer, offset + 8, CpuSubType);
            LittleEndianWriter.WriteUInt32(buffer, offset + 12, (uint)FileType);
            LittleEndianWriter.WriteUInt32(buffer, offset + 16, NumberOfLoadCommands);
            LittleEndianWriter.WriteUInt32(buffer, offset + 20, SizeOfLoadCommands);
            LittleEndianWriter.WriteUInt32(buffer, offset + 24, (uint)Flags);
            if (m_is64BitHeader)
            {
                LittleEndianWriter.WriteUInt32(buffer, offset + 28, Reserved);
            }
        }

        public int Length
        {
            get
            {
                if (m_is64BitHeader)
                {
                    return Length64Bit;
                }
                else
                {
                    return Length32Bit;
                }
            }
        }

        public bool Is64Bit
        {
            get
            {
                return m_is64BitHeader;
            }
        }

        public static bool IsMachHeader(byte[] buffer, int offset)
        {
            uint magic = BigEndianConverter.ToUInt32(buffer, offset);
            return (magic == MachO32BitLittleEndianSignature ||
                    magic == MachO64BitLittleEndianSignature);
        }
    }
}
