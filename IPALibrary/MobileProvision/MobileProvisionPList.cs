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
using System.IO;
using PListNet;
using PListNet.Nodes;
using Utilities;

namespace IPALibrary
{
    public class MobileProvisionPList : PListFile
    {
        public MobileProvisionPList(byte[] plistBytes) : base(plistBytes)
        {
        }

        public string AppIDName
        {
            get
            {
                return PListHelper.GetStringValueFromPList(RootNode, "AppIDName");
            }
        }

        public List<string> ApplicationIdentifierPrefix
        {
            get
            {
                return PListHelper.GetStringArrayValueFromPList(RootNode, "ApplicationIdentifierPrefix");
            }
        }

        public DateTime? CreationDate
        {
            get
            {
                return PListHelper.GetDateTimeValueFromPList(RootNode, "CreationDate");
            }
        }

        public DictionaryNode EntitlementsNode
        {
            get
            {
                return PListHelper.GetDictionaryValueFromPList(RootNode, "Entitlements");
            }
        }

        public EntitlementsFile Entitlements
        {
            get
            {
                DictionaryNode entitlementsNode = EntitlementsNode;
                if (entitlementsNode != null)
                {
                    return new EntitlementsFile(entitlementsNode);
                }
                return null;
            }
        }

        public List<byte[]> DeveloperCertificates
        {
            get
            {
                return PListHelper.GetDataArrayValueFromPList(RootNode, "DeveloperCertificates");
            }
        }

        public DateTime? ExpirationDate
        {
            get
            {
                return PListHelper.GetDateTimeValueFromPList(RootNode, "ExpirationDate");
            }
        }

        public string ProfileName
        {
            get
            {
                return PListHelper.GetStringValueFromPList(RootNode, "Name");
            }
        }

        public List<string> ProvisionedDevices
        {
            get
            {
                return PListHelper.GetStringArrayValueFromPList(RootNode, "ProvisionedDevices");
            }
        }
    }
}
