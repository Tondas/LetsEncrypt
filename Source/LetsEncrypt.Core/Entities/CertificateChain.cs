using System;
using System.Linq;

namespace LetsEncrypt.Core.Entities
{
    public class CertificateChain : BaseEntity
    {
        public string Content => UnknownContent;

        public string Certificate => Process().Item1;
        public byte[] CertificateBytes => GetBytesFromPem(Certificate);

        public string Issuer => Process().Item2;
        public byte[] IssuerBytes => GetBytesFromPem(Issuer);

        // Ctors

        public CertificateChain()
        {
        }

        public CertificateChain(string content)
        {
            UnknownContent = content;
        }

        // Private Methods

        private (string, string) Process()
        {
            var certificates = Content
                    .Split(new[] { "-----END CERTIFICATE-----" }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Select(c => c + "-----END CERTIFICATE-----");

            return (
                certificates.First(),
                certificates.Last());//.Skip(1).ToList());
        }

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
    }
}