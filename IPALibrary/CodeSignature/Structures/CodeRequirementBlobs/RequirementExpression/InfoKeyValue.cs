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
    public class InfoKeyValue : RequirementExpression
    {
        public string Key;
        public string Value;

        public InfoKeyValue(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public InfoKeyValue(byte[] buffer, ref int offset)
        {
            Key = ReadAnsiString(buffer, ref offset);
            Value = ReadAnsiString(buffer, ref offset);
        }

        public override void WriteBytes(byte[] buffer, ref int offset)
        {
            BigEndianWriter.WriteUInt32(buffer, ref offset, (uint)RequirementOperatorName.InfoKeyValue);
            WriteAnsiString(buffer, ref offset, Key);
            WriteAnsiString(buffer, ref offset, Value);
        }

        public override int Length
        {
            get
            {
                return 4 + GetAnsiStringLength(Key) + GetAnsiStringLength(Value);
            }
        }
    }
}