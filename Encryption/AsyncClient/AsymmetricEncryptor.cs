using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AsymmetricEncryptClient
{
    public class AsymmetricEncryptor
    {
        private RSACryptoServiceProvider rsa;

        private RSAParameters _rsaPrivateKey;
        
        private RSAParameters _rsaPublicKey;

        public AsymmetricEncryptor()
        {
            rsa = new RSACryptoServiceProvider();
            _rsaPrivateKey = rsa.ExportParameters(true);
        }

        public void SetPublicKey(string publicKey)
        {
            try
            {
                byte[] pKey = Convert.FromBase64String(publicKey); 
                using(MemoryStream ms = new MemoryStream(pKey, false))
                {
                    var rsaKeyParams = (RsaKeyParameters)PublicKeyFactory.CreateKey(ms);
                    _rsaPublicKey.Modulus = rsaKeyParams.Modulus.ToByteArrayUnsigned();
                    _rsaPublicKey.Exponent = rsaKeyParams.Exponent.ToByteArrayUnsigned();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public byte[] EncryptRSA(string message)
        {
            byte[] encryptedData = null;
            try
            {
                rsa.ImportParameters(_rsaPublicKey);
                encryptedData = rsa.Encrypt(Encoding.UTF8.GetBytes(message), true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return encryptedData;
        }

        public string DecryptRSA(byte[] message)
        {
            byte[] decryptedData = null;

            try
            {
                rsa.ImportParameters(_rsaPrivateKey);
                decryptedData = rsa.Decrypt(message, true);
                return Convert.ToBase64String(decryptedData);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }


    }
}
