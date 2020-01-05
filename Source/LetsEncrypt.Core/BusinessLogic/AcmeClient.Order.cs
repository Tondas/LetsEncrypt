using LetsEncrypt.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsEncrypt.Core
{
    public partial class AcmeClient
    {
        // Public Methods

        public async Task<Order> GetOrderAsync(Account account, Uri orderLocation)
        {
            var signedData = _jws.Sign(null, account.Location, orderLocation, Nonce);
            return await PostAsync<Order>(orderLocation, signedData);
        }

        public async Task<Order> NewOrderAsync(Account account, List<string> identifiers, DateTime? notBefore = null, DateTime? notAfter = null)
        {
            var data = new Order
            {
                Identifiers = identifiers
                    .Select(id => new Identifier { Type = IdentifierType.Dns, Value = id })
                    .ToArray(),
                NotBefore = notBefore,
                NotAfter = notAfter,
            };

            var signedData = _jws.Sign(data, account.Location, Directory.NewOrder, Nonce);
            return await PostAsync<Order>(Directory.NewOrder, signedData);
        }

        //public async Task<Authorization> DeactivateOrderAsync()
        //{
        //    var signedData = _jws.Sign(new Authorization { Status = AuthorizationStatus.Deactivated }, location);
        //    return await PostAsync<Authorization>(location, signedData);
        //}
    }
}