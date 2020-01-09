using LetsEncrypt.Core;
using LetsEncrypt.Core.Entities;
using NUnit.Framework;
using System.Threading.Tasks;

namespace LetsEncrypt.Test
{
    public class AccountUT : BaseUT
    {
        [Test]
        public async Task UseExisingUT()
        {
            var acmeClient = new AcmeClient();
            await acmeClient.InitAsync(EnviromentUri);
            var keyPair = acmeClient.GenerateKeyPair();
            var account = await acmeClient.NewAccountAsync(ContactEmail);

            // TODO: persist key + account location in text format

            acmeClient.UseKeyPair(keyPair);
            var freshAccount = await acmeClient.GetAccountAsync(account.Location);

            Assert.IsNotNull(freshAccount);
        }

        [Test]
        public async Task DeactivateUT()
        {
            var acmeClient = new AcmeClient();
            await acmeClient.InitAsync(EnviromentUri);
            acmeClient.GenerateKeyPair();
            var account = await acmeClient.NewAccountAsync(ContactEmail);

            account = await acmeClient.DeactivateAccountAsync(account);

            Assert.IsTrue(account.Status == AccountStatus.Deactivated);
        }
    }
}