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
    public class CmsSignatureBlob : CodeSignatureBlob
    {
        public const uint Signature = 0xfade0b01; // CSMAGIC_BLOBWRAPPER

        // uint Magic;
        // uint Length;
        public byte[] Data; // CMS Signature Data in ASN.1

        public CmsSignatureBlob()
        {
            Data = new byte[0];
        }

        public CmsSignatureBlob(byte[] buffer, int offset) : base(buffer, offset)
        {
            uint length = BigEndianConverter.ToUInt32(buffer, offset + 4);
            Data = ByteReader.ReadBytes(buffer, offset + 8, (int)length - 8);
        }

        public override void WriteBytes(byte[] buffer, int offset)
        {
            BigEndianWriter.WriteUInt32(buffer, offset + 0, Signature);
            BigEndianWriter.WriteUInt32(buffer, offset + 4, (uint)Length);
            ByteWriter.WriteBytes(buffer, offset + 8, Data);
        }

        public override int Length
        {
            get
            {
                return 8 + Data.Length;
            }
        }
    }
}
