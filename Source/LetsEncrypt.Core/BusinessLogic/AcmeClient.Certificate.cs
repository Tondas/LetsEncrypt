using LetsEncrypt.Core.Cryptography.Certificate;
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

        public async Task<byte[]> GenerateCertificateAsync(Account account, Order order, string certificatePassword, string certificateCommonName)
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
            var cerBuilder = new CertificateBuilder(certificatePassword, certificateCommonName, order.Identifiers.Select(i => i.Value).ToList());

            // Generate certificate request
            byte[] request = cerBuilder.CreateSigningRequest();

            // Send certificate to CA
            order = await Finalize(account, order, request);

            if (order.Status != OrderStatus.Valid)
            {
                throw new Exception("Fail during finalization of your order!");
            }

            // Download signed certificate
            var certificatePem = await Download(account, order);

            // Finalize certificate
            return cerBuilder.FinalizeCertificate(certificatePem);
        }

        // Private Methods

        private async Task<Order> Finalize(Account account, Order order, byte[] cert)
        {
            var orderCert = new OrderCertificate() { Csr = JwsConvert.ToBase64String(cert) };
            var signedData = _jws.Sign(orderCert, account.Location, order.Finalize, Nonce);
            return await PostAsync<Order>(order.Finalize, signedData);
        }

        private async Task<CertificateChain> Download(Account account, Order order)
        {
            var signedData = _jws.Sign(null, account.Location, order.Certificate, Nonce);
            return await PostAsync<CertificateChain>(order.Certificate, signedData);
        }
    }
}