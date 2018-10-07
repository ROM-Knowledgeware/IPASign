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
using Utilities;

namespace IPALibrary
{
    public class PListFile
    {
        private PListFormat m_format;
        public PNode RootNode;

        public PListFile()
        {
        }

        public PListFile(byte[] plistBytes)
        {
            m_format = PListHelper.DetectFormat(plistBytes);
            MemoryStream stream = new MemoryStream(plistBytes);
            RootNode = PList.Load(stream);
        }

        public byte[] GetBytes()
        {
            return GetBytes(m_format);
        }

        public byte[] GetBytes(PListFormat format)
        {
            MemoryStream result = new MemoryStream();
            PList.Save(RootNode, result, format);
            if (format == PListFormat.Xml)
            {
                result.Position = 0;
                string xml = new StreamReader(result).ReadToEnd();
                xml = RemoveUTF8BOM(xml);
                // iOS 10.3.3 Will crash the application if it's using '<false />' instead of '<false/>'
                // in the entitlements file embedded in the executable.
                xml = xml.Replace(" />", "/>");
#if MAX_PLIST_COMPATIBILITY
                // Optional: Use exactly the same XML declaration and indentation as XCode
                const string DesiredXmlDeclaration = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                if (xml.StartsWith(DesiredXmlDeclaration, StringComparison.OrdinalIgnoreCase))
                {
                    // PList produces by Apple have encoding set to "UTF-8" in uppercase letters.
                    xml = DesiredXmlDeclaration + xml.Substring(DesiredXmlDeclaration.Length);
                }

                string[] lines = xml.Split('\n');
                for(int index = 0; index < lines.Length; index++)
                {
                    if (lines[index].StartsWith("\t"))
                    {
                        lines[index] = lines[index].Substring(1);
                    }
                }
                xml = String.Join("\n", lines) + "\n";
#endif
                byte[] bytes = Encoding.UTF8.GetBytes(xml);
                return bytes;
            }
            return result.ToArray();
        }

        public static string RemoveUTF8BOM(string text)
        {
            return text.TrimStart('\uFEFF'); // Remove Byte Order Mark
        }
    }
}
