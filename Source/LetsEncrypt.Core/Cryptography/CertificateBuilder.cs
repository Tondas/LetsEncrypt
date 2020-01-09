using LetsEncrypt.Core.Entities;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace LetsEncrypt.Core.Cryptography
{
    public static class CertificateBuilder
    {
        // Public Methods

        public static byte[] CreateSigningRequest(RSA rsa, string cn, List<string> subjectAlternativeNames)
        {
            CertificateRequest req = new CertificateRequest($"CN={cn}",
                   rsa,
                   HashAlgorithmName.SHA256,
                   RSASignaturePadding.Pkcs1);

            req.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(true, false, 0, true));
            req.CertificateExtensions.Add(
                new X509SubjectKeyIdentifierExtension(req.PublicKey, false));

            var sanb = new SubjectAlternativeNameBuilder();
            foreach (var subjectAlternativeName in subjectAlternativeNames)
            {
                sanb.AddDnsName(subjectAlternativeName);
            }
            req.CertificateExtensions.Add(sanb.Build());

            return req.CreateSigningRequest();
        }

        public static byte[] Generate(RSA rsa, CertificateChain certificateChain, string password, X509ContentType certificateType)
        {
            var certificate = new X509Certificate2(certificateChain.CertificateBytes);
            var issuer = new X509Certificate2(certificateChain.IssuerBytes);

            certificate = certificate.CopyWithPrivateKey(rsa);

            var collection = new X509Certificate2Collection();
            collection.Add(certificate);
            collection.Add(issuer);
            return collection.Export(certificateType, password);
        }
    }
}