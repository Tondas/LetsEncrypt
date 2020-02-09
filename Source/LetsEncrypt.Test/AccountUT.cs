using LetsEncrypt.Client;
using LetsEncrypt.Client.Entities;
using NUnit.Framework;
using System.Threading.Tasks;

namespace LetsEncrypt.Test
{
    public class AccountUT : BaseUT
    {
        [Test, Order(1)]
        public void Dummy()
        {
            Assert.Pass();
        }

        [Test, Order(2)]
        public async Task AccountCreation()
        {
            var acmeClient = new AcmeClient(EnviromentUri);
            var account = await acmeClient.CreateNewAccountAsync(ContactEmail);

            Assert.IsNotNull(account);
        }

        [Test, Order(3)]
        public async Task AccountPersistance()
        {
            var acmeClient = new AcmeClient(EnviromentUri);
            var tempAccount = await acmeClient.CreateNewAccountAsync(ContactEmail);

            await tempAccount.SaveAsync();

            // Load Stored Account
            var account = await Account.LoadAsync(ContactEmail);

            Assert.IsNotNull(account);
        }
    }
}

/*
Then this newly created account can be persisted locally into `\Store` directory inside root of your application:
```cs
await acmeClient.StoreAccountLocallyAsync();
```

And next time instead of creation new account you can use your previously stored account:
```cs
await acmeClient.UseLocallyStoredAccountAsync("your@email.com");
```
*/