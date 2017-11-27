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

namespace IPALibrary.CodeSignature
{
    public class AnchorHash : RequirementExpression
    {
        public uint Slot;
        public byte[] Hash;

        public AnchorHash()
        {
        }

        public AnchorHash(byte[] buffer, ref int offset)
        {
            Slot = BigEndianReader.ReadUInt32(buffer, ref offset);
            Hash = ReadData(buffer, ref offset);
        }

        public override void WriteBytes(byte[] buffer, ref int offset)
        {
            BigEndianWriter.WriteUInt32(buffer, ref offset, (uint)RequirementOperatorName.AnchorHash);
            BigEndianWriter.WriteUInt32(buffer, ref offset, Slot);
            WriteData(buffer, ref offset, Hash);
        }

        public override int Length
        {
            get
            {
                return 8 + GetDataLength(Hash);
            }
        }
    }
}
