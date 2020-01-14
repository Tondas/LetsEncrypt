using LetsEncrypt.Core.Entities;
using LetsEncrypt.Core.Jws;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LetsEncrypt.Core
{
    public partial class AcmeClient
    {
        // Public Methods

        public async Task<Certificate> GenerateCertificateAsync(Account account, Order order, string certificateCommonName)
        {
            // Load fresh order
            order = await GetOrderAsync(account, order.Location);

            // Verify Status
            if (order.Status != OrderStatus.Ready &&
                order.Status != OrderStatus.Pending)
            {
                throw new Exception("Order status must be 'Ready' or 'Pending'!");
            }

            // Initialize builder
            var cert = new Certificate();

            // Generate certificate request
            byte[] request = cert.CreateSigningRequest(certificateCommonName, order.Identifiers.Select(i => i.Value).ToList());

            // Send certificate to CA
            order = await Finalize(account.Location, order, request);

            if (order.Status != OrderStatus.Valid)
            {
                throw new Exception("Fail during finalization of your order!");
            }

            // Download signed certificate
            var certificateChainPem = await Download(account.Location, order);

            cert.AddChain(certificateChainPem);

            return cert;
        }

        public async Task RevokeCertificateAsync(Certificate certificate, RevocationReason reason = RevocationReason.Unspecified)
        {
            var certificateRevocation = new CertificateRevocation
            {
                Certificate = JwsConvert.ToBase64String(certificate.GetOriginalCertificate()),
                Reason = reason
            };

            var signedData = new JwsSigner(certificate.Key).Sign(certificateRevocation, url: Directory.RevokeCert, nonce: Nonce);
            var result = await PostAsync<Empty>(Directory.RevokeCert, signedData);
        }

        // Private Methods

        private async Task<Order> Finalize(Uri accountLocation, Order order, byte[] cert)
        {
            var orderCert = new OrderCertificate() { Csr = JwsConvert.ToBase64String(cert) };
            var signedData = _jws.Sign(orderCert, accountLocation, order.Finalize, Nonce);
            return await PostAsync<Order>(order.Finalize, signedData);
        }

        private async Task<CertificateChain> Download(Uri accountLocation, Order order)
        {
            var signedData = _jws.Sign(null, accountLocation, order.Certificate, Nonce);
            return await PostAsync<CertificateChain>(order.Certificate, signedData);
        }
    }
}