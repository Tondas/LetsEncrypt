using LetsEncrypt.Core.Cryptography;
using LetsEncrypt.Core.Json;
using LetsEncrypt.Core.Jws;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace LetsEncrypt.Core
{
    public partial class AcmeClient : BaseAcmeClient
    {
        #region Fields + Properties

        private readonly JsonSerializerSettings _jsonSettings = JsonSettings.CreateSettings();
        private JwsSigner _jws;
        private RsaKeyPair RsaKeyPair { get; set; }
        public bool HasKeyPair => RsaKeyPair != null;

        #endregion Fields + Properties

        // Ctor

        public AcmeClient()
            : base()
        {
        }

        // Public Methods

        public RsaKeyPair GenerateKeyPair()
        {
            RsaKeyPair = RsaCryptoProvider.GenerateKeys();
            InitJwsSigner(RsaKeyPair);
            return RsaKeyPair;
        }

        public void UseKeyPair(RsaKeyPair rsaKeys)
        {
            RsaKeyPair = rsaKeys;
            InitJwsSigner(RsaKeyPair);
        }

        public void UseKeyPair(RSAParameters privateKey, RSAParameters publicKey)
        {
            UseKeyPair(new RsaKeyPair(privateKey, publicKey));
        }

        public void UseKeyPair(string privateKey, string publicKey)
        {
            UseKeyPair(new RsaKeyPair(privateKey, publicKey));
        }

        // Private Methods

        private void InitJwsSigner(RsaKeyPair keyPair)
        {
            _jws = new JwsSigner(keyPair);
        }
    }
}