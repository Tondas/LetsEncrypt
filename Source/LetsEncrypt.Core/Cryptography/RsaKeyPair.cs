using LetsEncrypt.Core.Jws;
using System.Security.Cryptography;

namespace LetsEncrypt.Core.Cryptography
{
    public class RsaKeyPair
    {
        #region Const + Fields + Properties

        public const string KEY_TYPE = "RSA";
        public const string THUMBPRINT_ALGORITHM_NAME = "SHA256";
        public readonly string ALGORITHM_NAME = "RS256";

        public RSAParameters Private { get; set; }
        public string PrivateString => RsaCryptoProvider.RSAKeyToString(Private);

        public RSAParameters Public { get; set; }
        public string PublicString => RsaCryptoProvider.RSAKeyToString(Public);

        public RsaJsonWebKey Jwk => ComposeJwk();

        #endregion Const + Fields + Properties

        // Ctor

        public RsaKeyPair()
        {
        }

        public RsaKeyPair(RSAParameters privateKey, RSAParameters publicKey)
            : this()
        {
            Private = privateKey;
            Public = publicKey;
        }

        public RsaKeyPair(string privateKey, string publicKey)
            : this()
        {
            Private = RsaCryptoProvider.RSAKeyFromString(privateKey);
            Public = RsaCryptoProvider.RSAKeyFromString(publicKey);
        }

        // Public Methods

        public byte[] SignData(byte[] data)
        {
            return RsaCryptoProvider.SignData(this, data);
        }

        public byte[] SignHash(byte[] hash)
        {
            return RsaCryptoProvider.SignHash(this, hash);
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

        //public byte[] ToDer()
        //        //{
        //        //    var privateKey = PrivateKeyInfoFactory.CreatePrivateKeyInfo(KeyPair.Private);
        //        //    return privateKey.GetDerEncoded();
        //        //}

        //        //public string ToPem()
        //        //{
        //        //    using (var sr = new StringWriter())
        //        //    {
        //        //        var pemWriter = new PemWriter(sr);
        //        //        pemWriter.WriteObject(KeyPair);
        //        //        return sr.ToString();
        //        //    }
        //        //}
    }
}