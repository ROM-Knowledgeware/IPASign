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
    public class EntitlementsBlob : CodeSignatureGenericBlob
    {
        public const uint Signature = 0xfade7171; // CSMAGIC_EMBEDDED_ENTITLEMENTS

        public EntitlementsBlob()
        {
            Magic = Signature;
        }

        public EntitlementsBlob(byte[] buffer, int offset) : base(buffer, offset)
        {
        }
    }
}
