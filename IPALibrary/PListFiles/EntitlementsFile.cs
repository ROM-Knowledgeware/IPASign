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

namespace IPALibrary
{
    public class EntitlementsFile : PListFile
    {
        public EntitlementsFile()
        {
        }

        public EntitlementsFile(PNode rootNode)
        {
            RootNode = rootNode;
        }

        public EntitlementsFile(byte[] entitlementsBytes) : base(entitlementsBytes)
        {
        }

        public string ApplicationIdentifier
        {
            get
            {
                return PListHelper.GetStringValueFromPList(RootNode, "application-identifier");
            }
        }

        public string TeamIdentifier
        {
            get
            {
                return PListHelper.GetStringValueFromPList(RootNode, "com.apple.developer.team-identifier");
            }
        }

        public string BundleIdentifier
        {
            get
            {
                string teamID = TeamIdentifier;
                string applicationID = ApplicationIdentifier;
                if (teamID != null && applicationID != null)
                {
                    if (applicationID.StartsWith(teamID) && applicationID.Length > teamID.Length)
                    {
                        return applicationID.Substring(teamID.Length + 1);
                    }
                }
                return null;
            }
        }
    }
}
