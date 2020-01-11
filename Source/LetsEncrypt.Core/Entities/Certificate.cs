using LetsEncrypt.Core.Cryptography;
using LetsEncrypt.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace LetsEncrypt.Core.Entities
{
    public class Certificate
    {
        #region Consts + Fields + Properties

        private const int KEY_SIZE = 2048;
        private readonly string _cn;
        private readonly SecureString _password;
        private readonly List<string> _subjectAlternativeNames;
        private CertificateChain _certificateChain;

        public RSA Rsa { get; private set; }

        #endregion Consts + Fields + Properties

        // Ctor

        public Certificate(string cn, List<string> subjectAlternativeNames, string password)
        {
            _cn = cn;
            _subjectAlternativeNames = subjectAlternativeNames;
            _password = password.ToSecureString();

            // Generate new RSA key for certificate
            Rsa = RSA.Create(KEY_SIZE);
        }

        // Public Methods

        public byte[] CreateSigningRequest()
        {
            return CertificateBuilder.CreateSigningRequest(Rsa, _cn, _subjectAlternativeNames);
        }

        public void AddChain(CertificateChain certificateChain)
        {
            _certificateChain = certificateChain;
        }

        public byte[] GeneratePfx()
        {
            return CertificateBuilder.Generate(Rsa, _certificateChain, _password.ToString2(), X509ContentType.Pfx);
        }

        public byte[] GenerateCrt()
        {
            return CertificateBuilder.Generate(Rsa, _certificateChain, _password.ToString2(), X509ContentType.Cert);
        }

        public string GenerateCrtPem()
        {
            return string.Format(
                "-----BEGIN CERTIFICATE-----\n{0}\n-----END CERTIFICATE-----",
                Convert.ToBase64String(GenerateCrt()));
        }

        public string GenerateKeyPem()
        {
            return string.Format(
                "-----BEGIN RSA PRIVATE KEY-----\n{0}\n-----END RSA PRIVATE KEY-----",
                Convert.ToBase64String(Rsa.ExportRSAPrivateKey()));
        }
    }
}