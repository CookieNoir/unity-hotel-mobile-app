using System;
using System.IO;
using System.Security.Cryptography;
using System.Linq;

namespace Hotel.Networking
{
    public class SecurityHelper
    {
        private const int _SALT_LENGTH = 32;
        private const int _ITERATIONS = 100;
        private const int _HASH_LENGTH = 64;

        public static string GenerateSalt()
        {
            var saltBytes = new byte[_SALT_LENGTH];

            using (var provider = new RNGCryptoServiceProvider())
            {
                provider.GetNonZeroBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);

            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, _ITERATIONS))
            {
                return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(_HASH_LENGTH));
            }
        }

        public static string EncryptDataWithAes(string plainText, out string keyBase64, out string vectorBase64)
        {
            using (Aes aesAlgorithm = Aes.Create())
            {
                keyBase64 = Convert.ToBase64String(aesAlgorithm.Key);
                vectorBase64 = Convert.ToBase64String(aesAlgorithm.IV);

                ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor();

                byte[] encryptedData;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        encryptedData = ms.ToArray();
                    }
                }

                return Convert.ToBase64String(encryptedData);
            }
        }
        public static string DecryptDataWithAes(string cipherText, string keyBase64, string vectorBase64)
        {
            using (Aes aesAlgorithm = Aes.Create())
            {
                aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
                aesAlgorithm.IV = Convert.FromBase64String(vectorBase64);

                ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor();

                byte[] cipher = Convert.FromBase64String(cipherText);

                using (MemoryStream ms = new MemoryStream(cipher))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}