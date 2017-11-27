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
using System.Text;
using PListNet;
using PListNet.Nodes;

namespace IPALibrary
{
    public class PListHelper
    {
        public static PListFormat DetectFormat(byte[] fileBytes)
        {
            // compare to known indicator
            if (Encoding.UTF8.GetString(fileBytes, 0, 8) == "bplist00")
            {
                return PListFormat.Binary;
            }
            else
            {
                return PListFormat.Xml;
            }
        }

        public static string GetStringValueFromPList(PNode rootNode, string key)
        {
            if (rootNode is DictionaryNode)
            {
                PNode value;
                if (((DictionaryNode)rootNode).TryGetValue(key, out value))
                {
                    if (value is StringNode)
                    {
                        return ((StringNode)value).Value;
                    }
                }
            }
            return null;
        }

        public static DateTime? GetDateTimeValueFromPList(PNode rootNode, string key)
        {
            if (rootNode is DictionaryNode)
            {
                PNode value;
                if (((DictionaryNode)rootNode).TryGetValue(key, out value))
                {
                    if (value is DateNode)
                    {
                        return ((DateNode)value).Value;
                    }
                }
            }
            return null;
        }

        public static byte[] GetDataValueFromPList(PNode rootNode, string key)
        {
            if (rootNode is DictionaryNode)
            {
                PNode value;
                if (((DictionaryNode)rootNode).TryGetValue(key, out value))
                {
                    if (value is DataNode)
                    {
                        return ((DataNode)value).Value;
                    }
                }
            }
            return null;
        }

        public static List<string> GetStringArrayValueFromPList(PNode rootNode, string key)
        {
            if (rootNode is DictionaryNode)
            {
                PNode value;
                if (((DictionaryNode)rootNode).TryGetValue(key, out value))
                {
                    if (value is ArrayNode)
                    {
                        ArrayNode array = (ArrayNode)value;
                        List<string> result = new List<string>();
                        foreach (PNode node in array)
                        {
                            StringNode stringNode = node as StringNode;
                            if (stringNode != null)
                            {
                                result.Add(stringNode.Value);
                            }
                        }
                        return result;
                    }
                }
            }
            return null;
        }

        public static List<byte[]> GetDataArrayValueFromPList(PNode rootNode, string key)
        {
            if (rootNode is DictionaryNode)
            {
                PNode value;
                if (((DictionaryNode)rootNode).TryGetValue(key, out value))
                {
                    if (value is ArrayNode)
                    {
                        ArrayNode array = (ArrayNode)value;
                        List<byte[]> result = new List<byte[]>();
                        foreach (PNode node in array)
                        {
                            DataNode dataNode = node as DataNode;
                            if (dataNode != null)
                            {
                                result.Add(dataNode.Value);
                            }
                        }
                        return result;
                    }
                }
            }
            return null;
        }

        public static DictionaryNode GetDictionaryValueFromPList(PNode rootNode, string key)
        {
            if (rootNode is DictionaryNode)
            {
                PNode value;
                if (((DictionaryNode)rootNode).TryGetValue(key, out value))
                {
                    if (value is DictionaryNode)
                    {
                        return (DictionaryNode)value;
                    }
                }
            }
            return null;
        }
    }
}
