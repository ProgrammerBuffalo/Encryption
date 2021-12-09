using SymmetricEncryptClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hacker
{
    class DecryptionUtil
    {
        public static string DecryptAES(byte[] data, string key)
        {
            Cypher cypher = deserializeEncryptedData(data);

            byte[] encryptedData = cypher.Data;
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
                        using (StreamReader readerStream = new StreamReader(cryptoStream))
                        {
                            decryptedData = readerStream.ReadToEnd();
                        }
                    }
                }

            }
            return decryptedData;
        }

        private static Cypher deserializeEncryptedData(byte[] data)
        {
            Cypher cypher = null;
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(data))
            {
                stream.Seek(0, SeekOrigin.Begin);
                cypher = binaryFormatter.Deserialize(stream) as Cypher;
            }
            return cypher;
        }
    }
}
