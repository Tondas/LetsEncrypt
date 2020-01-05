using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace LetsEncrypt.Core.Cryptography
{
    public class RsaCryptoProvider
    {
        private const int KEY_SIZE = 2048;

        //

        public static RsaKeyPair GenerateKeys()
        {
            var result = new RsaKeyPair();

            using (var rsa = new RSACryptoServiceProvider(KEY_SIZE))
            {
                result.Public = rsa.ExportParameters(false);
                result.Private = rsa.ExportParameters(true);
            }

            return result;
        }

        public static (string, string) GenerateKeys2()
        {
            var keyPair = GenerateKeys();
            return (keyPair.PrivateString, keyPair.PublicString);
        }

        //

        public static byte[] SignData(RsaKeyPair keyPair, byte[] data)
        {
            using (var rsa = new RSACryptoServiceProvider(KEY_SIZE))
            {
                rsa.ImportParameters(keyPair.Private);

                return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        public static byte[] SignHash(RsaKeyPair keyPair, byte[] hash)
        {
            using (var rsa = new RSACryptoServiceProvider(KEY_SIZE))
            {
                rsa.ImportParameters(keyPair.Private);

                return rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        //

        public static string Encrypt(RSAParameters publicKey, string dataToEncrypt)
        {
            using (var rsa = new RSACryptoServiceProvider(KEY_SIZE))
            {
                // Use public key to encrypt
                rsa.ImportParameters(publicKey);

                // Encrypt data
                var encyptedData = rsa.Encrypt(Encoding.UTF8.GetBytes(dataToEncrypt), true);

                // Return encrypted Base64 string
                return Convert.ToBase64String(encyptedData);
            }
        }

        public static string Encrypt(string publicKey, string dataToEncrypt)
        {
            return Encrypt(RSAKeyFromString(publicKey), dataToEncrypt);
        }

        //

        public static string Decrypt(RSAParameters privateKey, string encryptedData)
        {
            using (var rsa = new RSACryptoServiceProvider(KEY_SIZE))
            {
                // Use private key to decrypt
                rsa.ImportParameters(privateKey);

                // Get data from Base64 string
                var encryptedDataBytes = Convert.FromBase64String(encryptedData);

                // Decrypt data
                var decryptedData = rsa.Decrypt(encryptedDataBytes, true);

                // Return decrypted plain text
                return Encoding.UTF8.GetString(decryptedData);
            }
        }

        public static string Decrypt(string privateKey, string dataToEncrypt)
        {
            return Decrypt(RSAKeyFromString(privateKey), dataToEncrypt);
        }

        //

        public static string RSAKeyToString(RSAParameters key)
        {
            using (var writer = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(RSAParameters));
                serializer.Serialize(writer, key);
                return writer.ToString();
            }
        }

        public static RSAParameters RSAKeyFromString(string key)
        {
            using (var reader = new StringReader(key))
            {
                var serializer = new XmlSerializer(typeof(RSAParameters));
                return (RSAParameters)serializer.Deserialize(reader);
            }
        }
    }
}