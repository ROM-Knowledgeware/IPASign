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

namespace IPALibrary.MachO
{
    public class MachObjectHelper
    {
        public static List<MachObjectFile> ReadMachObjects(byte[] buffer)
        {
            if (UniversalBinaryFile.IsUniversalBinaryFile(buffer))
            {
                UniversalBinaryFile file = new UniversalBinaryFile(buffer);
                return file.MachObjects;
            }
            else if (MachObjectFile.IsMachObjectFile(buffer, 0))
            {
                List<MachObjectFile> result = new List<MachObjectFile>();
                result.Add(new MachObjectFile(buffer, 0, buffer.Length));
                return result;
            }
            else
            {
                return null;
            }
        }

        public static byte[] PackMachObjects(List<MachObjectFile> files)
        {
            if (files.Count == 1)
            {
                return files[0].GetBytes();
            }
            else
            {
                UniversalBinaryFile universalBinaryFile = new UniversalBinaryFile();
                universalBinaryFile.Header.NumberOfArchitectures = (uint)files.Count;
                foreach(MachObjectFile machObject in files)
                {
                    FatArch fatArch = new FatArch();
                    fatArch.CpuType = machObject.Header.CpuType;
                    fatArch.CpuSubType = machObject.Header.CpuSubType;
                    universalBinaryFile.Architectures.Add(fatArch, machObject);
                }
                return universalBinaryFile.GetBytes();
            }
        }
    }
}
