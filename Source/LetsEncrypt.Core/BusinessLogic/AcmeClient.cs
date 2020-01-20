using LetsEncrypt.Core.Cryptography;
using LetsEncrypt.Core.Entities;
using LetsEncrypt.Core.Json;
using LetsEncrypt.Core.Jws;
using Newtonsoft.Json;
using System;

namespace LetsEncrypt.Core
{
    public partial class AcmeClient : BaseAcmeClient
    {
        #region Fields + Properties

        private readonly JsonSerializerSettings _jsonSettings = JsonSettings.CreateSettings();
        private JwsSigner _jws;
        private RsaKeyPair Key { get; set; }
        private Account Account { get; set; }

        private bool HasKey => Key != null;
        private bool HasAccount => Account != null;

        #endregion Fields + Properties

        // Ctor

        public AcmeClient(Uri directoryUri)
            : base(directoryUri)
        {
        }

        // Private Methods

        private void GenerateNewKey()
        {
            Key = RsaKeyPair.New();
            InitJwsSigner(Key);
        }

        private void UseExistingKey(RsaKeyPair key)
        {
            Key = key;
            InitJwsSigner(Key);
        }

        private void InitJwsSigner(RsaKeyPair keyPair)
        {
            _jws = new JwsSigner(Key);
        }
    }
}