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
    public class BooleanValue : RequirementExpression
    {
        public bool Value;

        public BooleanValue(bool value)
        {
        }

        public override void WriteBytes(byte[] buffer, ref int offset)
        {
            if (Value)
            {
                BigEndianWriter.WriteUInt32(buffer, ref offset, (uint)RequirementOperatorName.True);
            }
            else
            {
                BigEndianWriter.WriteUInt32(buffer, ref offset, (uint)RequirementOperatorName.False);
            }
        }

        public override int Length
        {
            get
            {
                return 4;
            }
        }
    }
}
