using LetsEncrypt.Client.Entities;
using LetsEncrypt.Client.Jws;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LetsEncrypt.Client
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
            order = await Finalize(account, order, request);

            if (order.Status != OrderStatus.Valid)
            {
                throw new Exception("Fail during finalization of your order!");
            }

            // Download signed certificate
            var certificateChainPem = await Download(account, order);

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

            var directory = await GetDirectoryAsync();
            var nonce = await GetNonceAsync();

            var signedData = new JwsSigner(certificate.Key).Sign(certificateRevocation, url: directory.RevokeCert, nonce: nonce);
            var result = await PostAsync<Empty>(directory.RevokeCert, signedData);
        }

        // Private Methods

        private async Task<Order> Finalize(Account account, Order order, byte[] cert)
        {
            var nonce = await GetNonceAsync();

            var orderCert = new OrderCertificate() { Csr = JwsConvert.ToBase64String(cert) };
            var signedData = account.Signer.Sign(orderCert, account.Location, order.Finalize, nonce);
            return await PostAsync<Order>(order.Finalize, signedData);
        }

        private async Task<CertificateChain> Download(Account account, Order order)
        {
            var nonce = await GetNonceAsync();

            var signedData = account.Signer.Sign(null, account.Location, order.Certificate, nonce);
            return await PostAsync<CertificateChain>(order.Certificate, signedData);
        }
    }
}