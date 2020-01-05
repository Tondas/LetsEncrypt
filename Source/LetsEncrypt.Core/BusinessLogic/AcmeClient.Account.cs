using LetsEncrypt.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetsEncrypt.Core
{
    public partial class AcmeClient
    {
        // Public Methods

        public async Task<Account> NewAccountAsync(string contactEmail)
        {
            return await NewAccountAsync(new List<string> { $"mailto:{contactEmail}" });
        }

        public async Task<Account> GetAccountAsync(Uri accountLocation)
        {
            var signedData = _jws.Sign(null, accountLocation, accountLocation, Nonce);
            return await PostAsync<Account>(accountLocation, signedData);
        }

        public async Task<Account> DeactivateAccountAsync(Account account)
        {
            var data = new Account { Status = AccountStatus.Deactivated };
            var signedData = _jws.Sign(data, account.Location, account.Location, Nonce);
            return await PostAsync<Account>(account.Location, signedData);
        }

        public async Task<Account> UpdateAccountAsync(Account account)
        {
            var signedData = _jws.Sign(account, account.Location, account.Location, Nonce);
            return await PostAsync<Account>(account.Location, signedData);
        }

        // Private Methods

        private async Task<Account> NewAccountAsync(List<string> contactEmails)
        {
            var account = new Account
            {
                Contact = contactEmails,
                TermsOfServiceAgreed = true
            };
            var signedData = _jws.Sign(account, url: Directory.NewAccount, nonce: Nonce);
            return await PostAsync<Account>(Directory.NewAccount, signedData);
        }
    }
}