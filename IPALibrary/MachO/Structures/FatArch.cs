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
using Utilities;

namespace IPALibrary.MachO
{
    /// <summary>
    /// Describes the location within the binary of an object file targeted at a single architecture.
    /// Regardless of the content this data structure describes, all its fields are stored in big-endian byte order.
    /// </summary>
    public class FatArch
    {
        public const int Length = 20;

        public CpuType CpuType;
        public uint CpuSubType;
        public uint Offset;
        public uint Size;
        public uint AlignLog2;

        public FatArch()
        {
        }

        public FatArch(byte[] buffer, int offset)
        {
            CpuType = (CpuType)BigEndianConverter.ToUInt32(buffer, offset + 0);
            CpuSubType = BigEndianConverter.ToUInt32(buffer, offset + 4);
            Offset = BigEndianConverter.ToUInt32(buffer, offset + 8);
            Size = BigEndianConverter.ToUInt32(buffer, offset + 12);
            AlignLog2 = BigEndianConverter.ToUInt32(buffer, offset + 16);
        }

        public void WriteBytes(byte[] buffer, int offset)
        {
            BigEndianWriter.WriteUInt32(buffer, offset + 0, (uint)CpuType);
            BigEndianWriter.WriteUInt32(buffer, offset + 4, CpuSubType);
            BigEndianWriter.WriteUInt32(buffer, offset + 8, Offset);
            BigEndianWriter.WriteUInt32(buffer, offset + 12, Size);
            BigEndianWriter.WriteUInt32(buffer, offset + 16, AlignLog2);
        }

        public int Align
        {
            get
            {
                return 1 <<  (int)AlignLog2;
            }
            set
            {
                AlignLog2 = (byte)Math.Log(value, 2);
            }
        }
    }
}
