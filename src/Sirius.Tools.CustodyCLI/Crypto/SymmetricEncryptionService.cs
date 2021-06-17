using System;
using System.Linq;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Sirius.Tools.CustodyCLI.Crypto
{
    public class SymmetricEncryptionService
    {
        private const int KeyBitSize = 256;
        private const int MacBitSize = 128;
        private const int NonceBitSize = 128;

        private static readonly SecureRandom Random = new();

        public static byte[] Encrypt(byte[] data, byte[] key, byte[] nonce)
        {
            return Process(data, key, nonce, true);
        }

        public static byte[] Decrypt(byte[] data, byte[] key, byte[] nonce)
        {
            return Process(data, key, nonce, false);
        }

        public static byte[] GenerateKey()
        {
            var key = new byte[KeyBitSize / 8];
            Random.NextBytes(key);
            key[^1] &= 0x7F;
            return key;
        }

        public static byte[] GenerateNonce()
        {
            var nonce = new byte[NonceBitSize / 8];
            Random.NextBytes(nonce, 0, nonce.Length);
            return nonce;
        }

        private static byte[] Process(byte[] data, byte[] key, byte[] nonce, bool encrypt)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (nonce == null)
                throw new ArgumentNullException(nameof(nonce));

            if (data.Length == 0)
                throw new ArgumentException("Data required.", nameof(data));

            if (key.Length != KeyBitSize / 8)
                throw new ArgumentException($"Key should be {KeyBitSize} bit.", nameof(key));

            if (nonce.Length != NonceBitSize / 8)
                throw new ArgumentException($"Nonce needs to be {KeyBitSize} bit.", nameof(nonce));

            var cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()), new Pkcs7Padding());
            cipher.Init(encrypt, new ParametersWithIV(new KeyParameter(key), nonce));

            var buffer = new byte[cipher.GetOutputSize(data.Length)];
            var length = cipher.ProcessBytes(data, 0, data.Length, buffer, 0);

            length += cipher.DoFinal(buffer, length);

            return buffer.Take(length).ToArray();
        }
    }
}