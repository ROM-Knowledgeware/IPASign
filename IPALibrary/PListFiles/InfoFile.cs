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
using System.Security.Cryptography;
using System.IO;
using PListNet;
using PListNet.Nodes;

namespace IPALibrary
{
    public class InfoFile : PListFile
    {
        public InfoFile(byte[] infoBytes) : base(infoBytes)
        {
        }

        public string ExecutableName
        {
            get
            {
                return PListHelper.GetStringValueFromPList(RootNode, "CFBundleExecutable");
            }
        }

        public string BundleIdentifier
        {
            get
            {
                return PListHelper.GetStringValueFromPList(RootNode, "CFBundleIdentifier");
            }
            set
            {
                if (RootNode is DictionaryNode)
                {
                    StringNode stringNode = new StringNode(value);
                    ((DictionaryNode)RootNode)["CFBundleIdentifier"] = stringNode;
                }
            }
        }
    }
}
