using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace AuthDemo.Security
{
    public class Cypher
    {
        private readonly RSAParameters _publicKey;
        private readonly RSAParameters _privateKey;

        private byte[] _easKey;
        private byte[] _aesIv;

        private const int IterationCount = 1000;

        public Cypher()
        {
            var rsa = new RSACryptoServiceProvider(2048);

            _publicKey = rsa.ExportParameters(false);
            _privateKey = rsa.ExportParameters(true);
        }

        /// <summary>
        /// Value - user text
        /// </summary>
        /// <param name="value"></param>
        public byte[] Encrypt(byte[] value)
        {
            var salt = Generate256BitsOfRandomEntropy();
            var generatedKey = Generate256BitsOfRandomEntropy();

            using (var rfc = new Rfc2898DeriveBytes(generatedKey, salt, IterationCount))
            {
                var aes = new AesManaged
                {
                    KeySize = 256,
                    BlockSize = 128,
                    Mode = CipherMode.CBC
                };

                var keyBytes = rfc.GetBytes(aes.KeySize / 8);
                var ivBytes = rfc.GetBytes(aes.BlockSize / 8);

                aes.Key = keyBytes;
                aes.IV = ivBytes;

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

                        rfc.Reset();
                    }

                    EncyptAesKey(keyBytes, ivBytes);

                    return stream.ToArray();
                }
            }
        }

        public byte[] Decrypt(byte[] cypherText)
        {
            var rsa = new RSACryptoServiceProvider(2048);
            rsa.ImportParameters(_privateKey);

            var aesKey = rsa.Decrypt(_easKey, true);
            var aesIv = rsa.Decrypt(_aesIv, true);

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

        private  void EncyptAesKey(byte[] keyBytes, byte[] ivBytes)
        {
            try
            {
                var rsa = new RSACryptoServiceProvider(2048);
                rsa.ImportParameters(_publicKey);

                _easKey = rsa.Encrypt(keyBytes, true);
                _aesIv = rsa.Encrypt(ivBytes, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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