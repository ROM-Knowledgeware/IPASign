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
    public class CodeDirectoryHash : RequirementExpression
    {
        public byte[] Hash;

        public CodeDirectoryHash()
        {
        }

        public CodeDirectoryHash(byte[] buffer, ref int offset)
        {
            Hash = ReadData(buffer, ref offset);
        }

        public override void WriteBytes(byte[] buffer, ref int offset)
        {
            BigEndianWriter.WriteUInt32(buffer, ref offset, (uint)RequirementOperatorName.CodeDirectoryHash);
            WriteData(buffer, ref offset, Hash);
        }

        public override int Length
        {
            get
            {
                return 4 + GetDataLength(Hash);
            }
        }
    }
}
