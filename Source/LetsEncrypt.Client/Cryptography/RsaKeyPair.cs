using LetsEncrypt.Client.Extensions;
using LetsEncrypt.Client.Jws;
using System;
using System.Security.Cryptography;
using System.Text;

namespace LetsEncrypt.Client.Cryptography
{
    public class RsaKeyPair
    {
        #region Consts + Fields + Properties

        private const string RSA_PEM_STRING_PRIVATE = "RSA PRIVATE KEY";
        private const string RSA_PEM_STRING_PUBLIC = "RSA PUBLIC KEY";
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

        public RsaKeyPair(string privateKeyPem)
        {
            var privateKeyBytes = GetBytesFromPem(privateKeyPem, RSA_PEM_STRING_PRIVATE);

            Private = privateKeyBytes.CreateRsaParametersFromKeyBytes();
            Public = new RSAParameters()
            {
                Exponent = Private.Exponent,
                Modulus = Private.Modulus
            };
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
                "-----BEGIN {1}-----\n{0}\n-----END {1}-----",
                Convert.ToBase64String(this.ToRSA().ExportRSAPrivateKey()),
                RSA_PEM_STRING_PRIVATE);
        }

        public string ToPublicKeyPem()
        {
            return string.Format(
                "-----BEGIN {1}-----\n{0}\n-----END {1}-----",
                Convert.ToBase64String(this.ToRSA().ExportRSAPublicKey()),
                RSA_PEM_STRING_PUBLIC);
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

        private byte[] GetBytesFromPem(string pem, string headerFooterKey)
        {
            var header = $"-----BEGIN {headerFooterKey}-----";
            var footer = $"-----END {headerFooterKey}-----";

            var start = pem.IndexOf(header, StringComparison.Ordinal);
            if (start < 0)
                return null;

            start += header.Length;
            var end = pem.IndexOf(footer, start, StringComparison.Ordinal) - start;

            if (end < 0)
                return null;

            return Convert.FromBase64String(pem.Substring(start, end));
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