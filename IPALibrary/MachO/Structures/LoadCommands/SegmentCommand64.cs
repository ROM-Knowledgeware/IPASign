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
    /// segment_command_64
    /// </summary>
    public class SegmentCommand64 : SegmentCommand
    {
        public const int FixedSize = 72;

        // string SegmentName; // segname, 16 bytes
        public ulong VMAddress;
        public ulong VMSize;
        public ulong FileOffset;
        public ulong FileSize; // The number of bytes occupied by this segment on disk
        public uint MaxProt;
        public uint InitProt;
        // uint NumberOfSections; // nsects
        public uint Flags;
        public uint Reserved1;
        public uint Reserved2;
        public List<Section64> Sections = new List<Section64>();

        public SegmentCommand64() : base(LoadCommandType.Segment64, FixedSize)
        {
        }

        public SegmentCommand64(byte[] buffer, int offset) : base(buffer, offset)
        {
            SegmentName = ByteReader.ReadAnsiString(buffer, offset + 8, 16).Trim('\0');
            VMAddress = LittleEndianConverter.ToUInt64(buffer, offset + 24);
            VMSize = LittleEndianConverter.ToUInt64(buffer, offset + 32);
            FileOffset = LittleEndianConverter.ToUInt64(buffer, offset + 40);
            FileSize = LittleEndianConverter.ToUInt64(buffer, offset + 48);
            MaxProt = LittleEndianConverter.ToUInt32(buffer, offset + 56);
            InitProt = LittleEndianConverter.ToUInt32(buffer, offset + 60);
            uint numberOfSections = LittleEndianConverter.ToUInt32(buffer, offset + 64);
            Flags = LittleEndianConverter.ToUInt32(buffer, offset + 68);

            for (int index = 0; index < numberOfSections; index++)
            {
                Section64 section = new Section64(buffer, offset + FixedSize + index * Section64.Length);
                Sections.Add(section);
            }
        }

        public override void WriteBytes(byte[] buffer, int offset)
        {
            base.WriteBytes(buffer, offset);
            ByteWriter.WriteAnsiString(buffer, offset + 8, SegmentName, 16);
            LittleEndianWriter.WriteUInt64(buffer, offset + 24, VMAddress);
            LittleEndianWriter.WriteUInt64(buffer, offset + 32, VMSize);
            LittleEndianWriter.WriteUInt64(buffer, offset + 40, FileOffset);
            LittleEndianWriter.WriteUInt64(buffer, offset + 48, FileSize);
            LittleEndianWriter.WriteUInt32(buffer, offset + 56, MaxProt);
            LittleEndianWriter.WriteUInt32(buffer, offset + 60, InitProt);
            LittleEndianWriter.WriteUInt32(buffer, offset + 64, (uint)Sections.Count);
            LittleEndianWriter.WriteUInt32(buffer, offset + 68, Flags);

            for (int index = 0; index < Sections.Count; index++)
            {
                Sections[index].WriteBytes(buffer, offset + FixedSize + index * Section64.Length);
            }
        }

        public override uint CommandSize
        {
            get
            {
                return FixedSize + (uint)Sections.Count * Section64.Length;
            }
        }
    }
}
