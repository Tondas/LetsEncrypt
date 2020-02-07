using LetsEncrypt.Client.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsEncrypt.Client
{
    public partial class AcmeClient
    {
        // Public Methods

        public async Task<Order> GetOrderAsync(Account account, Uri orderLocation)
        {
            var nonce = await GetNonceAsync();

            var signedData = account.Signer.Sign(null, account.Location, orderLocation, nonce);
            return await PostAsync<Order>(orderLocation, signedData);
        }

        public async Task<Order> NewOrderAsync(Account account, List<string> identifiers)
        {
            var data = new Order
            {
                Identifiers = identifiers
                    .Select(id => new Identifier { Type = IdentifierType.Dns, Value = id })
                    .ToArray()
            };

            var directory = await GetDirectoryAsync();
            var nonce = await GetNonceAsync();

            var signedData = account.Signer.Sign(data, account.Location, directory.NewOrder, nonce);
            return await PostAsync<Order>(directory.NewOrder, signedData);
        }

        //public async Task<Authorization> DeactivateOrderAsync()
        //{
        //    var signedData = _jws.Sign(new Authorization { Status = AuthorizationStatus.Deactivated }, location);
        //    return await PostAsync<Authorization>(location, signedData);
        //}
    }
}