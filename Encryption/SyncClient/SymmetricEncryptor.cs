using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SyncClient
{
    class SymmetricEncryptor
    {
        private static string key = "BxG2xTHkhrYnLkmWy5Tf8wXQj4KZd1O9";

        public static async Task<Cypher> EcryptAES(string data)
        {
            byte[] encryptedData = null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            await streamWriter.WriteAsync(data);
                        }
                        encryptedData = memoryStream.ToArray();
                    }
                }

                return new Cypher { Data = Convert.ToBase64String(encryptedData), IV = aes.IV };
            }
        }

        public static async Task<string> DecryptAES(Cypher cypher)
        {
            byte[] encryptedData = Convert.FromBase64String(cypher.Data);
            string decryptedData = null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = cypher.IV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(encryptedData))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader readerStream = new StreamReader(cryptoStream, Encoding.UTF8))
                        {
                            decryptedData = await readerStream.ReadToEndAsync();
                        }
                    }
                }

            }
            return decryptedData;
        }
    }
}
