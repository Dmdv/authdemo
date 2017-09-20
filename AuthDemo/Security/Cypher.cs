using System;
using System.IO;
using System.Security.Cryptography;

namespace AuthDemo.Security
{
    public class Cypher
    {
        private readonly RSAParameters _publicKey;
        private readonly RSAParameters _privateKey;

        private byte[] _aesCyphroKey;
        private byte[] _aesCyphroIv;

        private const int KeySize = 2048;
        private const int IterationCount = 1000;

        public Cypher()
        {
            using (var rsa = new RSACryptoServiceProvider(KeySize))
            {
                _publicKey = rsa.ExportParameters(false);
                _privateKey = rsa.ExportParameters(true);
            }
        }

        /// <summary>
        /// Value - user text
        /// </summary>
        /// <param name="value"></param>
        public byte[] Encrypt(byte[] value)
        {
            var salt = Generate256BitsOfRandomEntropy();
            var generatedKey = Generate256BitsOfRandomEntropy();

            var aes = new AesManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.CBC
            };

            using (var rfc = new Rfc2898DeriveBytes(generatedKey, salt, IterationCount))
            {
                var keyBytes = rfc.GetBytes(aes.KeySize / 8);
                var ivBytes = rfc.GetBytes(aes.BlockSize / 8);

                aes.Key = keyBytes;
                aes.IV = ivBytes;

                rfc.Reset();

                EncyptAesKey(keyBytes, ivBytes);
            }

            using (var stream = new MemoryStream())
            {
                using (aes)
                {
                    using (var cryptoStream = new CryptoStream(
                        stream,
                        aes.CreateEncryptor(),
                        CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(value, 0, value.Length);
                        cryptoStream.FlushFinalBlock();
                        cryptoStream.Close();
                    }
                }

                return stream.ToArray();
            }
        }

        public byte[] Decrypt(byte[] cypherText)
        {
            var keys = DecryptAesKeys();

            var aesKey = keys.Item1;
            var aesIv = keys.Item2;

            var aes = new AesManaged
            {
                KeySize = 256,
                BlockSize = 128,
                Mode = CipherMode.CBC,
                Key = aesKey,
                IV = aesIv
            };

            using (var stream = new MemoryStream())
            {
                using (aes)
                {
                    using (var cryptoStream = new CryptoStream(
                        stream,
                        aes.CreateDecryptor(),
                        CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(cypherText, 0, cypherText.Length);
                        cryptoStream.FlushFinalBlock();
                        cryptoStream.Close();
                    }
                }

                return stream.ToArray();
            }
        }

        private Tuple<byte[], byte[]> DecryptAesKeys()
        {
            byte[] aesKey;
            byte[] aesIv;

            using (var rsa = new RSACryptoServiceProvider(KeySize))
            {
                rsa.ImportParameters(_privateKey);

                aesKey = rsa.Decrypt(_aesCyphroKey, true);
                aesIv = rsa.Decrypt(_aesCyphroIv, true);
            }

            return new Tuple<byte[], byte[]>(aesKey, aesIv);
        }

        private  void EncyptAesKey(byte[] keyBytes, byte[] ivBytes)
        {
            using (var rsa = new RSACryptoServiceProvider(KeySize))
            {
                rsa.ImportParameters(_publicKey);

                _aesCyphroKey = rsa.Encrypt(keyBytes, true);
                _aesCyphroIv = rsa.Encrypt(ivBytes, true);
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32];
            using (var cryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                cryptoServiceProvider.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}