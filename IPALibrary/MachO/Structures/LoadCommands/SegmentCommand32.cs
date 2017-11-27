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
    /// segment_command
    /// </summary>
    public class SegmentCommand32 : SegmentCommand
    {
        public const int FixedSize = 56;

        // string SegmentName; // segname, 16 bytes
        public uint VMAddress;
        public uint VMSize;
        public uint FileOffset;
        public uint FileSize; // The number of bytes occupied by this segment on disk
        public uint MaxProt;
        public uint InitProt;
        // uint NumberOfSections; // nsects
        public uint Flags;
        public List<Section32> Sections = new List<Section32>();

        public SegmentCommand32() : base(LoadCommandType.Segment, FixedSize)
        {
        }

        public SegmentCommand32(byte[] buffer, int offset) : base(buffer, offset)
        {
            SegmentName = ByteReader.ReadAnsiString(buffer, offset + 8, 16).Trim('\0');
            VMAddress = LittleEndianConverter.ToUInt32(buffer, offset + 24);
            VMSize = LittleEndianConverter.ToUInt32(buffer, offset + 28);
            FileOffset = LittleEndianConverter.ToUInt32(buffer, offset + 32);
            FileSize = LittleEndianConverter.ToUInt32(buffer, offset + 36);
            MaxProt = LittleEndianConverter.ToUInt32(buffer, offset + 40);
            InitProt = LittleEndianConverter.ToUInt32(buffer, offset + 44);
            uint numberOfSections = LittleEndianConverter.ToUInt32(buffer, offset + 48);
            Flags = LittleEndianConverter.ToUInt32(buffer, offset + 52);

            for (int index = 0; index < numberOfSections; index++)
            {
                Section32 section = new Section32(buffer, offset + FixedSize + index * Section32.Length);
                Sections.Add(section); 
            }
        }

        public override void WriteBytes(byte[] buffer, int offset)
        {
            base.WriteBytes(buffer, offset);
            ByteWriter.WriteAnsiString(buffer, offset + 8, SegmentName, 16);
            LittleEndianWriter.WriteUInt32(buffer, offset + 24, VMAddress);
            LittleEndianWriter.WriteUInt32(buffer, offset + 28, VMSize);
            LittleEndianWriter.WriteUInt32(buffer, offset + 32, FileOffset);
            LittleEndianWriter.WriteUInt32(buffer, offset + 36, FileSize);
            LittleEndianWriter.WriteUInt32(buffer, offset + 40, MaxProt);
            LittleEndianWriter.WriteUInt32(buffer, offset + 44, InitProt);
            LittleEndianWriter.WriteUInt32(buffer, offset + 48, (uint)Sections.Count);
            LittleEndianWriter.WriteUInt32(buffer, offset + 52, Flags);

            for (int index = 0; index < Sections.Count; index++)
            {
                Sections[index].WriteBytes(buffer, offset + FixedSize + index * Section32.Length);
            }
        }

        public override uint CommandSize
        {
            get
            {
                return FixedSize + (uint)Sections.Count * Section32.Length;
            }
        }
    }
}
