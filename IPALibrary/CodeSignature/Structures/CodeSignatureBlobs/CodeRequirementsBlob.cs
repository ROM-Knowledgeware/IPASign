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
    /// <summary>
    /// Implements the CS_SuperBlob structure which is defined in codesign.c
    /// https://opensource.apple.com/source/Security/Security-55471/sec/Security/Tool/codesign.c
    /// </summary>
    public class CodeRequirementsBlob : CodeSignatureBlob
    {
        public const uint Signature = 0xfade0c01; // CSMAGIC_REQUIREMENTS
        public const int FixedLength = 12;

        // uint Magic;
        // uint Length;
        // uint Count;
        public KeyValuePairList<SecurityRequirementType, CodeRequirementBlob> Entries = new KeyValuePairList<SecurityRequirementType, CodeRequirementBlob>();

        public CodeRequirementsBlob()
        {
        }

        public CodeRequirementsBlob(byte[] buffer, int offset) : base(buffer, offset)
        {
            uint length = BigEndianConverter.ToUInt32(buffer, offset + 4);
            uint count = BigEndianConverter.ToUInt32(buffer, offset + 8);
            for (int index = 0; index < count; index++)
            {
                SecurityRequirementType entryType = (SecurityRequirementType)BigEndianConverter.ToUInt32(buffer, offset + 12 + index * 8);
                uint entryOffset = BigEndianConverter.ToUInt32(buffer, offset + 12 + index * 8 + 4);
                CodeRequirementBlob blob = CodeRequirementBlob.ReadCodeRequirementBlob(buffer, offset + (int)entryOffset);
                Entries.Add(entryType, blob);
            }
        }

        public override void WriteBytes(byte[] buffer, int offset)
        {
            BigEndianWriter.WriteUInt32(buffer, offset + 0, Signature);
            BigEndianWriter.WriteUInt32(buffer, offset + 4, (uint)Length);
            BigEndianWriter.WriteUInt32(buffer, offset + 8, (uint)Entries.Count);
            int blobOffset = FixedLength + Entries.Count * 8;
            for (int index = 0; index < Entries.Count; index++)
            {
                BigEndianWriter.WriteUInt32(buffer, offset + 12 + index * 8, (uint)Entries[index].Key);
                BigEndianWriter.WriteUInt32(buffer, offset + 12 + index * 8 + 4, (uint)blobOffset);
                Entries[index].Value.WriteBytes(buffer, offset + blobOffset);
                blobOffset += Entries[index].Value.Length;
            }
        }

        public override int Length
        {
            get
            {
                int length = FixedLength + Entries.Count * 8;
                for (int index = 0; index < Entries.Count; index++)
                {
                    length += Entries[index].Value.Length;
                }
                return length;
            }
        }
    }
}
