using LetsEncrypt.Core;
using LetsEncrypt.Core.Entities;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetsEncrypt.Test
{
    public class InitUT : Startup

    {
        [Test]
        public async Task Test()
        {
            var acmeClient = new AcmeClient();

            // Specify which environment you want to use
            await acmeClient.InitAsync(Environment.LetsEncryptV2Staging);

            // Generate new RSA key pair or use existing
            acmeClient.GenerateKeyPair();

            var account = await acmeClient.NewAccountAsync("au@turingion.com");
            var freshAccount = await acmeClient.GetAccountAsync(account.Location);

            //account.TermsOfServiceAgreed = false;
            //var account3 = await acmeClient.UpdateAccountAsync(account);
            //var account4 = await acmeClient.DeactivateAccountAsync(account);

            var order = await acmeClient.NewOrderAsync(account, new List<string> { "turingion.com" });
            //var order = await acmeClient.NewOrderAsync(account, new List<string> { "turingion.com", "*.turingion.com" });
            //var freshOrder = await acmeClient.GetOrderAsync(account, order.Location);

            var challanges = await acmeClient.GetDnsChallenges(account, order);

            foreach (var challange in challanges)
            {
                var dnsKey = challange.VerificationKey;
                var dnsText = challange.VerificationValue;

                // Create DNS entry
                // _acme-challenge.turingion.com

                await acmeClient.ValidateChallengeAsync(account, challange);

                var freshChallange = await acmeClient.GetChallengeAsync(account, challange);
            }

            // TODO: generate certificate

            Assert.Pass();
        }
    }
}