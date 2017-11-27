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
using System.Text;
using Utilities;

namespace IPALibrary.CodeSignature
{
    public abstract class RequirementExpression
    {
        public abstract void WriteBytes(byte[] buffer, ref int offset);

        public abstract int Length
        {
            get;
        }

        public static string ReadAnsiString(byte[] buffer, ref int offset)
        {
            byte[] data = ReadData(buffer, ref offset);
            return ASCIIEncoding.GetEncoding(28591).GetString(data);
        }

        public static byte[] ReadData(byte[] buffer, ref int offset)
        {
            uint length = BigEndianReader.ReadUInt32(buffer, ref offset);
            byte[] data = ByteReader.ReadBytes(buffer, ref offset, (int)length);
            int padding = (4 - ((int)length % 4)) % 4;
            offset += padding;
            return data;
        }

        public static void WriteAnsiString(byte[] buffer, ref int offset, string value)
        {
            byte[] data = ASCIIEncoding.GetEncoding(28591).GetBytes(value);
            WriteData(buffer, ref offset, data);
        }

        public static void WriteData(byte[] buffer, ref int offset, byte[] data)
        {
            BigEndianWriter.WriteUInt32(buffer, ref offset, (uint)data.Length);
            ByteWriter.WriteBytes(buffer, ref offset, data);
            int padding = (4 - (data.Length % 4)) % 4;
            ByteWriter.WriteBytes(buffer, ref offset, new byte[padding]);
        }

        public static int GetAnsiStringLength(string value)
        {
            return 4 + (int)Math.Ceiling((double)value.Length / 4) * 4;
        }

        public static int GetDataLength(byte[] data)
        {
            return 4 + (int)Math.Ceiling((double)data.Length / 4) * 4;
        }

        public static RequirementExpression ReadExpression(byte[] buffer, ref int offset)
        {
            RequirementOperatorName opName = (RequirementOperatorName)BigEndianReader.ReadUInt32(buffer, ref offset);
            switch(opName)
            {
                case RequirementOperatorName.False:
                    return new BooleanValue(false);
                case RequirementOperatorName.True:
                    return new BooleanValue(true);
                case RequirementOperatorName.Ident:
                    return new IdentValue(buffer, ref offset);
                case RequirementOperatorName.AppleAnchor:
                    return new AppleAnchor();
                case RequirementOperatorName.AnchorHash:
                    return new AnchorHash(buffer, ref offset);
                case RequirementOperatorName.InfoKeyValue:
                    return new InfoKeyValue(buffer, ref offset);
                case RequirementOperatorName.And:
                    return new AndExpression(buffer, ref offset);
                case RequirementOperatorName.Or:
                    return new OrExpression(buffer, ref offset);
                case RequirementOperatorName.CodeDirectoryHash:
                    return new CodeDirectoryHash(buffer, ref offset);
                case RequirementOperatorName.Not:
                    return new NotExpression(buffer, ref offset);
                case RequirementOperatorName.InfoKeyField:
                    return new InfoKeyField(buffer, ref offset);
                case RequirementOperatorName.CertificateField:
                    return new CertificateField(buffer, ref offset);
                case RequirementOperatorName.TrustedCertificate:
                    return new TrustedCertificate(buffer, ref offset);
                case RequirementOperatorName.TrustedCertificates:
                    return new TrustedCertificates(buffer, ref offset);
                case RequirementOperatorName.CertificateGeneric:
                    return new CertificateGeneric(buffer, ref offset);
                case RequirementOperatorName.AppleGenericAnchor:
                    return new AppleGenericAnchor();
                default:
                    throw new NotImplementedException("Requirement operator not implemented");
            }
        }
    }
}
