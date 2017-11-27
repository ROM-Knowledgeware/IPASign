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

namespace IPALibrary.MachO
{
    public class CodeSignatureCommand : LoadCommand
    {
        public const int Size = 16;

        public uint DataOffset;
        public uint DataSize;

        public CodeSignatureCommand() : base(LoadCommandType.CodeSignature, Size)
        {
        }

        public CodeSignatureCommand(byte[] buffer, int offset) : base(buffer, offset)
        {
            DataOffset = LittleEndianConverter.ToUInt32(buffer, offset + 8);
            DataSize = LittleEndianConverter.ToUInt32(buffer, offset + 12);
        }

        public override void WriteBytes(byte[] buffer, int offset)
        {
            base.WriteBytes(buffer, offset);
            LittleEndianWriter.WriteUInt32(buffer, offset + 8, DataOffset);
            LittleEndianWriter.WriteUInt32(buffer, offset + 12, DataSize);
        }
    }
}
