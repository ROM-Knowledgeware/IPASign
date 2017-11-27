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
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.X509;

namespace IPALibrary.CodeSignature
{
    public class RSAHelper
    {
        public static byte[] DecryptSignature(byte[] signatureBytes, RSAParameters rsaParameters)
        {
            RsaKeyParameters publicKey = new RsaKeyParameters(false, new BigInteger(1, rsaParameters.Modulus), new BigInteger(rsaParameters.Exponent));
            IBufferedCipher cipher = CipherUtilities.GetCipher("RSA/NONE/PKCS1Padding");
            cipher.Init(false, publicKey);

            byte[] decrypted = cipher.DoFinal(signatureBytes);
            return decrypted;
        }
    }
}
