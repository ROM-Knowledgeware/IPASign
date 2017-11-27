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
    public class UniversalBinaryFile
    {
        public const int DefaultAlignment = 16384;

        public FatHeader Header;
        public KeyValuePairList<FatArch, MachObjectFile> Architectures = new KeyValuePairList<FatArch, MachObjectFile>();
        
        public UniversalBinaryFile()
        {
            Header = new FatHeader();
        }

        public UniversalBinaryFile(byte[] buffer)
        {
            Header = new FatHeader(buffer);
            
            List<FatArch> architectures = new List<FatArch>();
            for (int index = 0; index < Header.NumberOfArchitectures; index++)
            {
                FatArch architecture = new FatArch(buffer, FatHeader.Length + index * FatArch.Length);
                architectures.Add(architecture);
            }

            foreach (FatArch architecture in architectures)
            {
                MachObjectFile machObject = new MachObjectFile(buffer, (int)architecture.Offset, (int)architecture.Size);
                Architectures.Add(architecture, machObject);
            }
        }

        public List<MachObjectFile> MachObjects
        {
            get
            {
                return Architectures.Values;
            }
        }

        public byte[] GetBytes()
        {
            foreach (FatArch fatArch in Architectures.Keys)
            {
                if (fatArch.AlignLog2 == 0)
                {
                    fatArch.Align = DefaultAlignment;
                }
            }

            byte[] buffer = new byte[Length];
            Header.WriteBytes(buffer);
            int nextObjectOffset = FatHeader.Length + Architectures.Count * FatArch.Length;
            for(int index = 0; index < Architectures.Count; index++)
            {
                FatArch fatArch = Architectures[index].Key;
                MachObjectFile file = Architectures[index].Value;
                int align = fatArch.Align;
                int padding = (align - (nextObjectOffset % align)) % align;
                nextObjectOffset += padding;
                fatArch.Offset = (uint)nextObjectOffset;
                fatArch.Size = (uint)file.Length;
                fatArch.WriteBytes(buffer, FatHeader.Length + index * FatArch.Length);
                file.WriteBytes(buffer, nextObjectOffset);
                nextObjectOffset += file.Length;
            }
            return buffer;
        }

        public int Length
        {
            get
            {
                int length = FatHeader.Length + Architectures.Count * FatArch.Length;
                int nextObjectOffset = length;
                for (int index = 0; index < Architectures.Count; index++)
                {
                    FatArch fatArch = Architectures[index].Key;
                    MachObjectFile file = Architectures[index].Value;
                    int align = fatArch.Align;
                    int padding = (align - (nextObjectOffset % align)) % align;
                    length += padding;
                    length += file.Length;
                    nextObjectOffset = length;
                }
                return length;
            }
        }

        public static bool IsUniversalBinaryFile(byte[] buffer)
        {
            return FatHeader.IsFatHeader(buffer);
        }
    }
}
