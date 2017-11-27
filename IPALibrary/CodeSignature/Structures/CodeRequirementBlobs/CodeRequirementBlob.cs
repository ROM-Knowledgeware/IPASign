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
    public class CodeRequirementBlob
    {
        public const uint Signature = 0xfade0c00; // CSMAGIC_REQUIREMENT
        public const uint CodeRequirementKind = 1;

        // uint Magic;
        // uint Length;
        public uint Kind;
        public RequirementExpression Expression;

        public CodeRequirementBlob()
        {
            Kind = CodeRequirementKind;
        }

        public CodeRequirementBlob(byte[] buffer, int offset)
        {
            uint length = BigEndianConverter.ToUInt32(buffer, offset + 4);
            Kind = BigEndianConverter.ToUInt32(buffer, offset + 8);
            offset += 12;
            Expression = RequirementExpression.ReadExpression(buffer, ref offset);
        }

        public void WriteBytes(byte[] buffer, int offset)
        {
            BigEndianWriter.WriteUInt32(buffer, ref offset, Signature);
            BigEndianWriter.WriteUInt32(buffer, ref offset, (uint)Length);
            BigEndianWriter.WriteUInt32(buffer, ref offset, Kind);
            Expression.WriteBytes(buffer, ref offset);
        }

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[Length];
            WriteBytes(buffer, 0);
            return buffer;
        }

        public int Length
        {
            get
            {
                return 12 + Expression.Length;
            }
        }

        public static CodeRequirementBlob ReadCodeRequirementBlob(byte[] buffer, int offset)
        {
            uint magic = BigEndianConverter.ToUInt32(buffer, offset);
            switch (magic)
            {
                case Signature:
                    return new CodeRequirementBlob(buffer, offset);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
