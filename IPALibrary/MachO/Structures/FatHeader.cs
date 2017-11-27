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
    public class FatHeader
    {
        public const int Length = 8;
        public const uint FatSignature = 0xcafebabe;

        // uint Magic;
        public uint NumberOfArchitectures; // nfat_arch

        public FatHeader()
        {
        }

        public FatHeader(byte[] buffer)
        {
            NumberOfArchitectures = BigEndianConverter.ToUInt32(buffer, 4);
        }

        public void WriteBytes(byte[] buffer)
        {
            BigEndianWriter.WriteUInt32(buffer, 0, FatSignature);
            BigEndianWriter.WriteUInt32(buffer, 4, NumberOfArchitectures);
        }

        internal static bool IsFatHeader(byte[] buffer)
        {
            uint magic = BigEndianConverter.ToUInt32(buffer, 0);
            return (magic == FatSignature);
        }
    }
}
