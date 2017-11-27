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
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace IPALibrary
{
    public class MemoryStreamDataSource : IStaticDataSource
    {
        private MemoryStream m_stream;

        public MemoryStreamDataSource(MemoryStream stream)
        {
            m_stream = stream;
        }

        public MemoryStreamDataSource(byte[] data)
        {
            m_stream = new MemoryStream(data);
        }

        public Stream GetSource()
        {
            return m_stream;
        }
    }
}
