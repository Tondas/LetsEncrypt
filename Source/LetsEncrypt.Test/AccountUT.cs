//using LetsEncrypt.Core;
//using LetsEncrypt.Core.Entities;
//using NUnit.Framework;
//using System.Threading.Tasks;

//namespace LetsEncrypt.Test
//{
//    public class AccountUT : BaseUT
//    {
//        [Test]
//        public async Task UseExisingUT()
//        {
//            var acmeClient = new AcmeClient(EnviromentUri);
//            var account = await acmeClient.NewAccountAsync(ContactEmail);

//            // TODO: persist key + account location in text format

//            Assert.IsNotNull(freshAccount);
//        }

//        [Test]
//        public async Task DeactivateUT()
//        {
//            var acmeClient = new AcmeClient(EnviromentUri);
//            var account = await acmeClient.NewAccountAsync(ContactEmail);

//            account = await acmeClient.DeactivateAccountAsync(account);

//            Assert.IsTrue(account.Status == AccountStatus.Deactivated);
//        }
//    }
//}

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