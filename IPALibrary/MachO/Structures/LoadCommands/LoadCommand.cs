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
    public class LoadCommand
    {
        private LoadCommandType m_commandType;
        private uint m_commandSize;
        public byte[] Data; // LoadCommand data, only used for unrecognized LoadCommand structures

        public LoadCommand(LoadCommandType commandType, uint commandSize)
        {
        }

        public LoadCommand(byte[] buffer, int offset)
        {
            m_commandType = (LoadCommandType)LittleEndianConverter.ToUInt32(buffer, offset + 0);
            m_commandSize = LittleEndianConverter.ToUInt32(buffer, offset + 4);
            if (this.GetType() == typeof(LoadCommand)) // We check if the current class is LoadCommand and not a class that inherits from LoadCommand
            {
                Data = ByteReader.ReadBytes(buffer, offset + 8, (int)(CommandSize - 8));
            }
        }

        public virtual void WriteBytes(byte[] buffer, int offset)
        {
            LittleEndianWriter.WriteUInt32(buffer, offset + 0, (uint)m_commandType);
            LittleEndianWriter.WriteUInt32(buffer, offset + 4, m_commandSize);
            if (this.GetType() == typeof(LoadCommand)) // We check if the current class is LoadCommand and not a class that inherits from LoadCommand
            {
                ByteWriter.WriteBytes(buffer, offset + 8, Data);
            }
        }

        public LoadCommandType CommandType
        {
            get
            {
                return m_commandType;
            }
        }

        public virtual uint CommandSize
        {
            get
            {
                return m_commandSize;
            }
        }

        public static LoadCommand ReadCommand(byte[] buffer, int offset)
        {
            LoadCommandType commandType = (LoadCommandType)LittleEndianConverter.ToUInt32(buffer, offset + 0);
            switch (commandType)
            {
                case LoadCommandType.Segment:
                    return new SegmentCommand32(buffer, offset + 0);
                case LoadCommandType.Segment64:
                    return new SegmentCommand64(buffer, offset + 0);
                case LoadCommandType.CodeSignature:
                    return new CodeSignatureCommand(buffer, offset + 0);
                default:
                     return new LoadCommand(buffer, offset + 0);
            }
        }
    }
}
