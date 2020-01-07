using LetsEncrypt.Core.Entities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace LetsEncrypt.Core.Cryptography.Certificate
{
    public class CertificateBuilder
    {
        #region Consts + Fields

        private const int KEY_SIZE = 2048;
        private readonly string _password;
        private readonly string _cn;
        private readonly List<string> _subjectAlternativeNames;
        private readonly RSA _rsaKey;

        #endregion Consts + Fields

        // Ctor

        public CertificateBuilder(string password, string cn, List<string> subjectAlternativeNames)
        {
            _password = password;
            _cn = cn;
            _subjectAlternativeNames = subjectAlternativeNames;
            _rsaKey = RSA.Create(KEY_SIZE);
        }

        // Public Methods

        public byte[] CreateSigningRequest()
        {
            CertificateRequest req = new CertificateRequest($"CN={_cn}",
                   _rsaKey,
                   HashAlgorithmName.SHA256,
                   RSASignaturePadding.Pkcs1);

            req.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(true, false, 0, true));
            req.CertificateExtensions.Add(
                new X509SubjectKeyIdentifierExtension(req.PublicKey, false));

            // SAN
            var sanb = new SubjectAlternativeNameBuilder();
            foreach (var subjectAlternativeName in _subjectAlternativeNames)
            {
                sanb.AddDnsName(subjectAlternativeName);
            }
            req.CertificateExtensions.Add(sanb.Build());

            return req.CreateSigningRequest();
        }

        public byte[] FinalizeCertificate(CertificateChain certificateChain)
        {
            var certificate = new X509Certificate2(GetBytesFromPem(certificateChain.Certificate));
            var issuer = new X509Certificate2(GetBytesFromPem(certificateChain.Issuer));

            certificate = certificate.CopyWithPrivateKey(_rsaKey);

            var collection = new X509Certificate2Collection();
            collection.Add(certificate);
            collection.Add(issuer);
            return collection.Export(X509ContentType.Pfx, _password);
        }

        // Private Helper Methods

        private byte[] GetBytesFromPem(string pem)
        {
            var header = "-----BEGIN CERTIFICATE-----";
            var footer = "-----END CERTIFICATE-----";

            var start = pem.IndexOf(header, StringComparison.Ordinal);
            if (start < 0)
                return null;

            start += header.Length;
            var end = pem.IndexOf(footer, start, StringComparison.Ordinal) - start;

            if (end < 0)
                return null;

            return Convert.FromBase64String(pem.Substring(start, end));
        }

        //private static string PemEncodeSigningRequest(CertificateRequest request, X509SignatureGenerator generator)
        //{
        //    byte[] pkcs10 = request.CreateSigningRequest(generator);
        //    StringBuilder builder = new StringBuilder();

        //    builder.AppendLine("-----BEGIN CERTIFICATE REQUEST-----");

        //    string base64 = Convert.ToBase64String(pkcs10);

        //    int offset = 0;
        //    const int LineLength = 64;

        //    while (offset < base64.Length)
        //    {
        //        int lineEnd = Math.Min(offset + LineLength, base64.Length);
        //        builder.AppendLine(base64.Substring(offset, lineEnd - offset));
        //        offset = lineEnd;
        //    }

        //    builder.AppendLine("-----END CERTIFICATE REQUEST-----");
        //    return builder.ToString();
        //}
    }
}