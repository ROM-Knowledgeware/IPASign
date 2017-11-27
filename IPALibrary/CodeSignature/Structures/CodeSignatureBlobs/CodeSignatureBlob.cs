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
    public abstract class CodeSignatureBlob
    {
        public CodeSignatureBlob()
        {
        }

        public CodeSignatureBlob(byte[] buffer, int offset)
        {
        }

        public abstract void WriteBytes(byte[] buffer, int offset);

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[Length];
            WriteBytes(buffer, 0);
            return buffer;
        }

        public abstract int Length
        {
            get;
        }

        public static CodeSignatureBlob ReadBlob(byte[] buffer, int offset)
        {
            uint magic = BigEndianConverter.ToUInt32(buffer, offset);
            switch (magic)
            {
                case CodeDirectoryBlob.Signature:
                    return new CodeDirectoryBlob(buffer, offset);
                case CodeRequirementsBlob.Signature:
                    return new CodeRequirementsBlob(buffer, offset);
                case EntitlementsBlob.Signature:
                    return new EntitlementsBlob(buffer, offset);
                case CmsSignatureBlob.Signature:
                    return new CmsSignatureBlob(buffer, offset);
                default:
                    return new CodeSignatureGenericBlob(buffer, offset);
            }
        }
    }
}
