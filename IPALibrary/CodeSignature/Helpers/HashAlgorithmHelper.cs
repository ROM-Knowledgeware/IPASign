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
using Utilities;

namespace IPALibrary.CodeSignature
{
    public class HashAlgorithmHelper
    {
        private const int SHA1Length = 20;
        private const int SHA256Length = 32;
        private const int SHA256TruncatedLength = 20;

        public static int GetHashLength(HashType hashType)
        {
            switch (hashType)
            {
                case HashType.SHA1:
                    return SHA1Length;
                case HashType.SHA256:
                    return SHA256Length;
                case HashType.SHA256Truncated:
                    return SHA256TruncatedLength;
                default:
                    throw new NotImplementedException("Unsupported HashType");
            }
        }

        private static HashAlgorithm CreateHashAlgorithm(HashType hashType)
        {
            switch (hashType)
            {
                case HashType.SHA1:
                    return SHA1Managed.Create();
                case HashType.SHA256:
                case HashType.SHA256Truncated:
                    return SHA256Managed.Create();
                default:
                    throw new NotImplementedException("Unsupported HashType");
            }
        }

        public static byte[] ComputeHash(HashType hashType, byte[] data)
        {
            return ComputeHash(hashType, data, 0, data.Length);
        }

        public static byte[] ComputeHash(HashType hashType, byte[] data, int offset, int length)
        {
            HashAlgorithm hashAlgorithm = CreateHashAlgorithm(hashType);
            byte[] hash = hashAlgorithm.ComputeHash(data, offset, length);
            if (hashType == HashType.SHA256Truncated)
            {
                hash = ByteReader.ReadBytes(hash, 0, SHA256TruncatedLength);
            }
            return hash;
        }

        public static List<byte[]> ComputeHashes(HashType hashType, int pageSize, byte[] data)
        {
            HashAlgorithm hashAlgorithm = CreateHashAlgorithm(hashType);   

            List<byte[]> hashes = new List<byte[]>();
            for(int offset = 0; offset < data.Length; offset += pageSize)
            {
                int remaining = data.Length - offset;
                int length = Math.Min(remaining, pageSize);
                byte[] hash = ComputeHash(hashType, data, offset, length);
                hashes.Add(hash);
            }
            return hashes;
        }
    }
}
