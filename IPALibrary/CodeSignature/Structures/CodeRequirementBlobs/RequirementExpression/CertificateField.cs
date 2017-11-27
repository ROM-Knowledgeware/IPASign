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
    public class CertificateField : RequirementExpression
    {
        public uint CertificateIndex;
        public string FieldName;
        public MatchSuffix Match;

        public CertificateField()
        {
        }

        public CertificateField(byte[] buffer, ref int offset)
        {
            CertificateIndex = BigEndianReader.ReadUInt32(buffer, ref offset);
            FieldName = ReadAnsiString(buffer, ref offset);
            Match = new MatchSuffix(buffer, ref offset);
        }

        public override void WriteBytes(byte[] buffer, ref int offset)
        {
            BigEndianWriter.WriteUInt32(buffer, ref offset, (uint)RequirementOperatorName.CertificateField);
            BigEndianWriter.WriteUInt32(buffer, ref offset, CertificateIndex);
            WriteAnsiString(buffer, ref offset, FieldName);
            Match.WriteBytes(buffer, ref offset);
        }

        public override int Length
        {
            get
            {
                return 8 + GetAnsiStringLength(FieldName) + Match.Length;
            }
        }
    }
}
