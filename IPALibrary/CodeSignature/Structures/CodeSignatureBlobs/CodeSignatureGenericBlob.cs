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
    /// Implements the CS_GenericBlob structure which is defined in codesign.c
    /// https://opensource.apple.com/source/Security/Security-55471/sec/Security/Tool/codesign.c
    /// </summary>
    public class CodeSignatureGenericBlob : CodeSignatureBlob
    {
        public uint Magic;
        // uint Length;
        public byte[] Data;

        public CodeSignatureGenericBlob()
        {
            Data = new byte[0];
        }

        public CodeSignatureGenericBlob(byte[] buffer, int offset) : base(buffer, offset)
        {
            Magic = BigEndianConverter.ToUInt32(buffer, offset + 0);
            uint length = BigEndianConverter.ToUInt32(buffer, offset + 4);
            Data = ByteReader.ReadBytes(buffer, offset + 8, (int)length - 8);
        }

        public override void WriteBytes(byte[] buffer, int offset)
        {
            BigEndianWriter.WriteUInt32(buffer, offset + 0, Magic);
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
