using LetsEncrypt.Client.Cryptography;
using LetsEncrypt.Client.Json;
using Newtonsoft.Json;
using System;
using System.Text;

namespace LetsEncrypt.Client.Jws
{
    public class JwsSigner
    {
        private readonly RsaKeyPair _key;

        // Ctor

        public JwsSigner(RsaKeyPair key)
        {
            _key = key;
        }

        // Public Methods

        public JwsData Sign(object data, Uri url, string nonce)
        {
            var header =
                new
                {
                    alg = _key.ALGORITHM_NAME,
                    jwk = _key.Jwk,
                    nonce,
                    url,
                };

            return Sign(header, data);
        }

        public JwsData Sign(object data, Uri kId, Uri url, string nonce)
        {
            var header =
                new
                {
                    alg = _key.ALGORITHM_NAME,
                    kid = kId,
                    nonce,
                    url,
                };

            return Sign(header, data);
        }

        // Private Methods

        private JwsData Sign(object header, object body)
        {
            var jsonSettings = JsonSettings.CreateSettings();
            var entityJson = body == null ?
                "" :
                JsonConvert.SerializeObject(body, Formatting.None, jsonSettings);
            var protectedHeaderJson = JsonConvert.SerializeObject(header, Formatting.None, jsonSettings);

            var entityEncoded = JwsConvert.ToBase64String(Encoding.UTF8.GetBytes(entityJson));
            var protectedHeaderEncoded = JwsConvert.ToBase64String(Encoding.UTF8.GetBytes(protectedHeaderJson));

            var signature = $"{protectedHeaderEncoded}.{entityEncoded}";
            var signatureBytes = Encoding.UTF8.GetBytes(signature);
            var signedSignatureBytes = _key.SignData(signatureBytes);
            var signedSignatureEncoded = JwsConvert.ToBase64String(signedSignatureBytes);

            return new JwsData
            {
                Protected = protectedHeaderEncoded,
                Payload = entityEncoded,
                Signature = signedSignatureEncoded
            };
        }
    }
}