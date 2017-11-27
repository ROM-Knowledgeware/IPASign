/* Copyright (C) 2017 ROM Knowledgeware. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 * 
 * Maintainer: Tal Aloni <tal@kmrom.com>
 */

namespace IPALibrary.MachO
{
    public abstract class SegmentCommand : LoadCommand
    {
        public string SegmentName; // segname, 16 bytes

        public SegmentCommand(LoadCommandType commandType, uint commandSize) : base(commandType, commandSize)
        {
        }

        public SegmentCommand(byte[] buffer, int offset) : base(buffer, offset)
        {
        }
    }
}
