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
    public class CertificateGeneric : RequirementExpression
    {
        public uint CertificateIndex;
        public byte[] OID;
        public MatchSuffix Match;

        public CertificateGeneric()
        {
        }

        public CertificateGeneric(byte[] buffer, ref int offset)
        {
            CertificateIndex = BigEndianReader.ReadUInt32(buffer, ref offset);
            OID = ReadData(buffer, ref offset);
            Match = new MatchSuffix(buffer, ref offset);
        }

        public override void WriteBytes(byte[] buffer, ref int offset)
        {
            BigEndianWriter.WriteUInt32(buffer, ref offset, (uint)RequirementOperatorName.CertificateGeneric);
            BigEndianWriter.WriteUInt32(buffer, ref offset, CertificateIndex);
            WriteData(buffer, ref offset, OID);
            Match.WriteBytes(buffer, ref offset);
        }

        public override int Length
        {
            get 
            {
                return 8 + GetDataLength(OID) + Match.Length;
            }
        }
    }
}
