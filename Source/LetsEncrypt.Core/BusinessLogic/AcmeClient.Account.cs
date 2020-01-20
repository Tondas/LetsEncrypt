using LetsEncrypt.Core.Cryptography;
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

        public async Task CreateNewAccountAsync(string contactEmail)
        {
            GenerateNewKey();

            if (!HasKey)
            {
                throw new Exception("Key is not initialize! Please create new account or use existing!");
            }

            Account = await NewAccountAsync(new List<string> { $"mailto:{contactEmail}" });
        }

        public async Task UseLocallyStoredAccountAsync(string contactEmail)
        {
            var accountPersisted = await LoadAccountPersisted(contactEmail);

            // Init key
            var key = new RsaKeyPair(accountPersisted.PrivateKeyPem, accountPersisted.PublicKeyPem);
            UseExistingKey(key);

            // Init account
            Account = await GetAccountAsync(new Uri(accountPersisted.AccountLocation));
        }

        public async Task StoreAccountLocallyAsync()
        {
            var accountPersisted = new AccountPersisted();
            accountPersisted.PrivateKeyPem = Key.ToPrivateKeyPem();
            accountPersisted.PublicKeyPem = Key.ToPublicKeyPem();
            accountPersisted.AccountLocation = Account.Location.AbsoluteUri;
            accountPersisted.AccountContactEmail = Account.Contact.FirstOrDefault();

            await SaveAccountPersisted(accountPersisted);
        }

        // Private Methods

        private async Task<Account> GetAccountAsync(Uri accountLocation)
        {
            var signedData = _jws.Sign(null, accountLocation, accountLocation, Nonce);
            var account = await PostAsync<Account>(accountLocation, signedData);
            account.Location = accountLocation;
            return account;
        }

        private async Task<Account> DeactivateAccountAsync(Account account)
        {
            var data = new Account { Status = AccountStatus.Deactivated };
            var signedData = _jws.Sign(data, account.Location, account.Location, Nonce);
            return await PostAsync<Account>(account.Location, signedData);
        }

        private async Task<Account> UpdateAccountAsync(Account account)
        {
            var signedData = _jws.Sign(account, account.Location, account.Location, Nonce);
            return await PostAsync<Account>(account.Location, signedData);
        }

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

        private async Task SaveAccountPersisted(AccountPersisted account)
        {
            // TODO: Save
        }

        private async Task<AccountPersisted> LoadAccountPersisted(string contactEmail)
        {
            // TODO: Load
            return new AccountPersisted();
        }
    }
}