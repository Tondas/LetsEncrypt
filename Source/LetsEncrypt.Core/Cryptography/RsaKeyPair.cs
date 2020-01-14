using LetsEncrypt.Core.Jws;
using System;
using System.Security.Cryptography;
using System.Text;

namespace LetsEncrypt.Core.Cryptography
{
    public class RsaKeyPair
    {
        #region Consts + Fields + Properties

        public const int KEY_SIZE = 2048;
        public const string KEY_TYPE = "RSA";
        public const string THUMBPRINT_ALGORITHM_NAME = "SHA256";
        public readonly string ALGORITHM_NAME = "RS256";

        public RSAParameters Private { get; set; }
        public RSAParameters Public { get; set; }
        public RsaJsonWebKey Jwk => ComposeJwk();

        #endregion Consts + Fields + Properties

        // Ctor

        public RsaKeyPair(RSAParameters privateKey, RSAParameters publicKey)
        {
            Private = privateKey;
            Public = publicKey;
        }

        // Public Methods

        public byte[] SignData(byte[] data)
        {
            using (var rsa = new RSACryptoServiceProvider(KEY_SIZE))
            {
                rsa.ImportParameters(this.Private);

                return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        public byte[] SignHash(byte[] hash)
        {
            using (var rsa = new RSACryptoServiceProvider(KEY_SIZE))
            {
                rsa.ImportParameters(this.Private);

                return rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        public string Encrypt(string dataToEncrypt)
        {
            using (var rsa = new RSACryptoServiceProvider(KEY_SIZE))
            {
                // Use public key to encrypt
                rsa.ImportParameters(this.Public);

                // Encrypt data
                var encyptedData = rsa.Encrypt(Encoding.UTF8.GetBytes(dataToEncrypt), true);

                // Return encrypted Base64 string
                return Convert.ToBase64String(encyptedData);
            }
        }

        public string Decrypt(string encryptedData)
        {
            using (var rsa = new RSACryptoServiceProvider(KEY_SIZE))
            {
                // Use private key to decrypt
                rsa.ImportParameters(this.Private);

                // Get data from Base64 string
                var encryptedDataBytes = Convert.FromBase64String(encryptedData);

                // Decrypt data
                var decryptedData = rsa.Decrypt(encryptedDataBytes, true);

                // Return decrypted plain text
                return Encoding.UTF8.GetString(decryptedData);
            }
        }

        public RSA ToRSA()
        {
            return RSA.Create(Private);
        }

        public string ToPrivateKeyPem()
        {
            return string.Format(
                "-----BEGIN RSA PRIVATE KEY-----\n{0}\n-----END RSA PRIVATE KEY-----",
                Convert.ToBase64String(this.ToRSA().ExportRSAPrivateKey()));
        }

        public string ToPublicKeyPem()
        {
            return string.Format(
                "-----BEGIN RSA PUBLIC KEY-----\n{0}\n-----END RSA PUBLIC KEY-----",
                Convert.ToBase64String(this.ToRSA().ExportRSAPublicKey()));
        }

        // Private Methods

        private RsaJsonWebKey ComposeJwk()
        {
            return new RsaJsonWebKey
            {
                KeyType = KEY_TYPE,
                Exponent = JwsConvert.ToBase64String(Public.Exponent),
                Modulus = JwsConvert.ToBase64String(Public.Modulus)
            };
        }

        // Static Methods

        public static RsaKeyPair New()
        {
            using (var rsa = new RSACryptoServiceProvider(KEY_SIZE))
            {
                return new RsaKeyPair(rsa.ExportParameters(true), rsa.ExportParameters(false));
            }
        }
    }
}