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
using Org.BouncyCastle.Asn1;

namespace IPALibrary
{
    public class Asn1Helper
    {
        // https://stackoverflow.com/a/32328582/3419770
        public static Asn1Object FindAsn1Value(string oid, Asn1Object obj)
        {
            Asn1Object result = null;
            if (obj is Asn1Sequence)
            {
                bool foundOID = false;
                foreach (Asn1Object entry in (Asn1Sequence)obj)
                {
                    DerObjectIdentifier derOID = entry as DerObjectIdentifier;
                    if (derOID != null && derOID.Id == oid)
                    {
                        foundOID = true;
                    }
                    else if (foundOID)
                    {
                        return entry;
                    }
                    else
                    {
                        result = FindAsn1Value(oid, entry);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }
            else if (obj is DerTaggedObject)
            {
                result = FindAsn1Value(oid, ((DerTaggedObject)obj).GetObject());
                if (result != null)
                {
                    return result;
                }
            }
            else
            {
                if (obj is DerSet)
                {
                    foreach (Asn1Object entry in (DerSet)obj)
                    {
                        result = FindAsn1Value(oid, entry);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }
            return null;
        }
    }
}
