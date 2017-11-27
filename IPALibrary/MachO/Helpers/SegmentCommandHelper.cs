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
    public class SegmentCommandHelper
    {
        public const string LinkEditSegmentName = "__LINKEDIT";

        public static SegmentCommand FindLinkEditSegment(List<LoadCommand> loadCommands)
        {
            foreach (LoadCommand loadCommand in loadCommands)
            {
                if (loadCommand is SegmentCommand)
                {
                    SegmentCommand segmentCommand = (SegmentCommand)loadCommand;
                    if (segmentCommand.SegmentName == LinkEditSegmentName)
                    {
                        return segmentCommand;
                    }
                }
            }
            return null;
        }

        public static void SetEndOffset(SegmentCommand segmentCommand, uint endOffset)
        {
            if (segmentCommand is SegmentCommand32)
            {
                SegmentCommand32 segmentCommand32 = (SegmentCommand32)segmentCommand;
                segmentCommand32.FileSize = endOffset - segmentCommand32.FileOffset;
            }
            else if (segmentCommand is SegmentCommand64)
            {
                SegmentCommand64 segmentCommand64 = (SegmentCommand64)segmentCommand;
                segmentCommand64.FileSize = endOffset - segmentCommand64.FileOffset;
            }
        }
    }
}
