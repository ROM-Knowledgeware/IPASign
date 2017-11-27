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
    public class IdentValue : RequirementExpression
    {
        public string Value;

        public IdentValue(string value)
        {
            Value = value;
        }

        public IdentValue(byte[] buffer, ref int offset)
        {
            Value = ReadAnsiString(buffer, ref offset);
        }

        public override void WriteBytes(byte[] buffer, ref int offset)
        {
            BigEndianWriter.WriteUInt32(buffer, ref offset, (uint)RequirementOperatorName.Ident);
            WriteAnsiString(buffer, ref offset, Value);
        }

        public override int Length
        {
            get
            {
                return 4 + GetAnsiStringLength(Value);
            }
        }
    }
}
