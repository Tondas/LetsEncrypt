using LetsEncrypt.Client.Cryptography;
using LetsEncrypt.Client.IO;
using LetsEncrypt.Client.Jws;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsEncrypt.Client.Entities
{
    public partial class Account
    {
        private static readonly LocalStorage _localStorage = new LocalStorage();

        [JsonIgnore]
        public RsaKeyPair Key { get; private set; }

        [JsonIgnore]
        public JwsSigner Signer { get; private set; }

        // Ctors

        public Account()
        {
            TermsOfServiceAgreed = true;
        }

        public Account(RsaKeyPair key)
            : this()
        {
            Key = key;
            Signer = new JwsSigner(Key);
        }

        public Account(RsaKeyPair key, string location)
            : this(key)
        {
            Location = new Uri(location);
        }

        // Public Methods

        public async Task SaveAsync()
        {
            var contactEmail = Contact.FirstOrDefault().Replace(Constants.PREFIX_MAILTO, string.Empty);

            await _localStorage.PersistAccount(contactEmail, this.Location.AbsoluteUri);
            await _localStorage.PersistPrivateKey(contactEmail, Key.ToPrivateKeyPem());
            //await _localStorage.PersistPublicKey(contactEmail, Key.ToPublicKeyPem());
        }

        public void FillBy(Account account)
        {
            this.UnknownContent = account.UnknownContent;
            this.Location = account.Location;
            this.Error = account.Error;
            this.Status = account.Status;
            this.Contact = account.Contact;
            this.TermsOfServiceAgreed = account.TermsOfServiceAgreed;
            this.InitialIp = account.InitialIp;
            this.CreatedAt = account.CreatedAt;
        }

        // Static Methods

        public static Account Create(List<string> contactEmails)
        {
            var key = RsaKeyPair.New();

            return new Account(key)
            {
                Contact = contactEmails
            };
        }

        public static async Task<Account> LoadAsync(string contactEmail)
        {
            var location = await _localStorage.LoadAccount(contactEmail);
            var privateKeyPem = await _localStorage.LoadPrivateKey(contactEmail);
            //var publicKeyPem = await _localStorage.LoadPublicKey(contactEmail);

            var key = new RsaKeyPair(privateKeyPem);

            return new Account(key, location);
        }
    }
}