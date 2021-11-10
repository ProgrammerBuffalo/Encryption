using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AsymmetricEncryptClient
{
    public class AsymmetricEncryptor
    {
        public static byte[] EncryptRSA(string message)
        {
            byte[] encryptedData = null;

            CspParameters cspParameters = new CspParameters();
            cspParameters.KeyContainerName = Guid.NewGuid().ToString();
            cspParameters.Flags = CspProviderFlags.CreateEphemeralKey;

            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    encryptedData = rsa.Encrypt(Encoding.UTF8.GetBytes(message), true);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return encryptedData;
        }

        public static string DecryptRSA(byte[] message)
        {
            byte[] decryptedData = null;

            CspParameters cspParameters = new CspParameters();
            cspParameters.KeyContainerName = Guid.NewGuid().ToString();
            cspParameters.Flags = CspProviderFlags.CreateEphemeralKey;

            try
            {
                using(RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    decryptedData = rsa.Decrypt(message, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return Convert.ToBase64String(decryptedData);
        }


    }
}
