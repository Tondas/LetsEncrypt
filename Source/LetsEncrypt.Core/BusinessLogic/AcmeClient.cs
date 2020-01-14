using LetsEncrypt.Core.Cryptography;
using LetsEncrypt.Core.Json;
using LetsEncrypt.Core.Jws;
using Newtonsoft.Json;

namespace LetsEncrypt.Core
{
    public partial class AcmeClient : BaseAcmeClient
    {
        #region Fields + Properties

        private readonly JsonSerializerSettings _jsonSettings = JsonSettings.CreateSettings();
        private JwsSigner _jws;
        private RsaKeyPair Key { get; set; }

        #endregion Fields + Properties

        // Ctor

        public AcmeClient()
            : base()
        {
        }

        // Public Methods

        public RsaKeyPair GenerateKeyPair()
        {
            Key = RsaKeyPair.New();
            InitJwsSigner(Key);
            return Key;
        }

        public void UseKeyPair(RsaKeyPair key)
        {
            Key = key;
            InitJwsSigner(Key);
        }

        // Private Methods

        private void InitJwsSigner(RsaKeyPair keyPair)
        {
            _jws = new JwsSigner(keyPair);
        }
    }
}