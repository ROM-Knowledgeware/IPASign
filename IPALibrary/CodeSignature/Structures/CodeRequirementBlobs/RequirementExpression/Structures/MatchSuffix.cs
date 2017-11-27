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
    public class MatchSuffix
    {
        public MatchOperationName MatchOperation;
        public string MatchValue;

        public MatchSuffix(MatchOperationName matchOperation)
        {
            MatchOperation = matchOperation;
        }

        public MatchSuffix(MatchOperationName matchOperation, string matchValue)
        {
            MatchOperation = matchOperation;
            MatchValue = matchValue;
        }

        public MatchSuffix(byte[] buffer, ref int offset)
        {
            MatchOperation = (MatchOperationName)BigEndianReader.ReadUInt32(buffer, ref offset);
            if (MatchOperation != MatchOperationName.Exists)
            {
                MatchValue = RequirementExpression.ReadAnsiString(buffer, ref offset);
            }
        }

        public void WriteBytes(byte[] buffer, ref int offset)
        {
            BigEndianWriter.WriteUInt32(buffer, ref offset, (uint)MatchOperation);
            if (MatchOperation != MatchOperationName.Exists)
            {
                RequirementExpression.WriteAnsiString(buffer, ref offset, MatchValue);
            }
        }

        public int Length
        {
            get
            {
                int length = 4;
                if (MatchOperation != MatchOperationName.Exists)
                {
                    length += RequirementExpression.GetAnsiStringLength(MatchValue);
                }
                return length;
            }
        }
    }
}
