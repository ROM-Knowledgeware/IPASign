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
    public class MachObjectFile
    {
        public MachHeader Header;
        public List<LoadCommand> LoadCommands = new List<LoadCommand>();
        public int DataOffset;
        public byte[] Data;

        public MachObjectFile()
        {
        }

        public MachObjectFile(byte[] buffer, int offset, int length)
        {
            Header = new MachHeader(buffer, offset);
            offset += Header.Length;
            for (int index = 0; index < Header.NumberOfLoadCommands; index++)
            {
                LoadCommand command = LoadCommand.ReadCommand(buffer, offset);
                LoadCommands.Add(command);
                offset += (int)command.CommandSize;
            }
            int dataLength = (int)(length - Header.Length - Header.SizeOfLoadCommands);
            DataOffset = (int)(Header.Length + Header.SizeOfLoadCommands);
            Data = ByteReader.ReadBytes(buffer, offset, dataLength);
        }

        public LoadCommand GetLoadCommand(LoadCommandType commandType)
        {
            foreach (LoadCommand command in LoadCommands)
            {
                if (command.CommandType == commandType)
                {
                    return command;
                }
            }
            return null;
        }

        public byte[] GetCodeSignatureBytes()
        {
            CodeSignatureCommand command = GetLoadCommand(LoadCommandType.CodeSignature) as CodeSignatureCommand;
            if (command != null)
            {
                return ByteReader.ReadBytes(Data, (int)command.DataOffset - DataOffset, (int)command.DataSize);
            }
            return null;
        }

        public void WriteBytes(byte[] buffer, int offset)
        {
            Header.WriteBytes(buffer, offset + 0);
            offset += Header.Length;
            foreach (LoadCommand command in LoadCommands)
            {
                command.WriteBytes(buffer, offset);
                offset += (int)command.CommandSize;
            }
            ByteWriter.WriteBytes(buffer, offset, Data);
        }

        public byte[] GetBytes()
        {
            byte[] buffer = new byte[Length];
            WriteBytes(buffer, 0);
            return buffer;
        }

        public int Length
        {
            get
            {
                int length = Header.Length;
                foreach (LoadCommand command in LoadCommands)
                {
                    length += (int)command.CommandSize;
                }
                length += Data.Length;
                return length;
            }
        }

        public static bool IsMachObjectFile(byte[] buffer, int offset)
        {
            return MachHeader.IsMachHeader(buffer, offset);
        }
    }
}
