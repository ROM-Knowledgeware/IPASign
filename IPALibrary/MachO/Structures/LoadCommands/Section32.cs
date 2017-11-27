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
    /// section
    /// </summary>
    public class Section32
    {
        public const int Length = 68;

        public string SectionName; // sectname, 16 bytes
        public string SegmentName; // segname, 16 bytes
        public uint Address;
        public uint Size;
        public uint Offset;
        public uint Align;
        public uint RelocationOffset; // reloff;
        public uint NumberOfRelocationOffsets;
        public uint Flags;
        public uint Reserved1;
        public uint Reserved2;

        public Section32()
        {
        }

        public Section32(byte[] buffer, int offset)
        {
            SectionName = ByteReader.ReadAnsiString(buffer, offset + 0, 16).Trim('\0');
            SegmentName = ByteReader.ReadAnsiString(buffer, offset + 16, 16).Trim('\0');
            Address = LittleEndianConverter.ToUInt32(buffer, offset + 32);
            Size = LittleEndianConverter.ToUInt32(buffer, offset + 36);
            Offset = LittleEndianConverter.ToUInt32(buffer, offset + 40);
            Align = LittleEndianConverter.ToUInt32(buffer, offset + 44);
            RelocationOffset = LittleEndianConverter.ToUInt32(buffer, offset + 48);
            NumberOfRelocationOffsets = LittleEndianConverter.ToUInt32(buffer, offset + 52);
            Flags = LittleEndianConverter.ToUInt32(buffer, offset + 56);
            Reserved1 = LittleEndianConverter.ToUInt32(buffer, offset + 60);
            Reserved2 = LittleEndianConverter.ToUInt32(buffer, offset + 64);
        }

        public void WriteBytes(byte[] buffer, int offset)
        {
            ByteWriter.WriteAnsiString(buffer, offset + 0, SectionName, 16);
            ByteWriter.WriteAnsiString(buffer, offset + 16, SegmentName, 16);
            LittleEndianWriter.WriteUInt32(buffer, offset + 32, Address);
            LittleEndianWriter.WriteUInt32(buffer, offset + 36, Size);
            LittleEndianWriter.WriteUInt32(buffer, offset + 40, Offset);
            LittleEndianWriter.WriteUInt32(buffer, offset + 44, Align);
            LittleEndianWriter.WriteUInt32(buffer, offset + 48, RelocationOffset);
            LittleEndianWriter.WriteUInt32(buffer, offset + 52, NumberOfRelocationOffsets);
            LittleEndianWriter.WriteUInt32(buffer, offset + 56, Flags);
            LittleEndianWriter.WriteUInt32(buffer, offset + 60, Reserved1);
            LittleEndianWriter.WriteUInt32(buffer, offset + 64, Reserved2);
        }
    }
}
