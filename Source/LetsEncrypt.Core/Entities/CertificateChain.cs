using System;
using System.Linq;

namespace LetsEncrypt.Core.Entities
{
    public class CertificateChain : BaseEntity
    {
        public string Certificate => Process().Item1;

        public string Issuer => Process().Item2;

        // Private Methods
        private (string, string) Process()
        {
            var certificates = UnknownContent
                    .Split(new[] { "-----END CERTIFICATE-----" }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Select(c => c + "-----END CERTIFICATE-----");

            return (
                certificates.First(),
                certificates.Last());//.Skip(1).ToList());
        }
    }
}