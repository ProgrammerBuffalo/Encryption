using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SymmetricEncryptClient
{
    class SymmetricEncryptor
    {
        public static Cypher EcryptAES(string data, string key)
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
                            streamWriter.Write(data);
                        }
                        encryptedData = memoryStream.ToArray();
                    }
                }

                return new Cypher { Data = encryptedData, IV = aes.IV };
            }
        }

        public static string DecryptAES(Cypher cypher, string key)
        {
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


        public static byte[] SerializeEncryptedData(Cypher cypher)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                binaryFormatter.Serialize(stream, cypher);
                stream.Seek(0, SeekOrigin.Begin);
                byte[] bytes = stream.ToArray();
                stream.Flush();
                return bytes;
            }
        }

        public static Cypher DeserializeEncryptedData(byte[] data)
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
