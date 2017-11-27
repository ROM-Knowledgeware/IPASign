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
using Org.BouncyCastle.Asn1;
using PListNet;
using PListNet.Nodes;
using Utilities;

namespace IPALibrary
{
    public class MobileProvisionFile
    {
        public const string PListOID = "1.2.840.113549.1.7.1";

        private Asn1Object m_rootObject;
        private MobileProvisionPList m_plist;

        public MobileProvisionFile(byte[] fileBytes)
        {
            Asn1InputStream stream = new Asn1InputStream(fileBytes);
            m_rootObject = Asn1Object.FromByteArray(fileBytes);
            byte[] plistBytes = GetPListBytes(m_rootObject);
            m_plist = new MobileProvisionPList(plistBytes);
        }

        public static byte[] GetPListBytes(Asn1Object rootObject)
        {
            Asn1Object plistObj = Asn1Helper.FindAsn1Value(PListOID, rootObject);
            if (plistObj is DerTaggedObject)
            {
                DerOctetString value = ((DerTaggedObject)plistObj).GetObject() as DerOctetString;
                if (value != null)
                {
                    return value.GetOctets();
                }
            }
            return null;
        }

        public MobileProvisionPList PList
        {
            get
            {
                return m_plist;
            }
        }
    }
}
