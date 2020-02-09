using LetsEncrypt.Client;
using LetsEncrypt.Client.Entities;
using NUnit.Framework;
using System.Threading.Tasks;

namespace LetsEncrypt.Test
{
    public class OrderUT : BaseUT
    {
        [Test, Order(1)]
        public async Task OrderCreation()
        {
            // Create client alias core object + specify which environment you want to use
            var acmeClient = new AcmeClient(EnviromentUri);

            // Create new Account
            var account = await acmeClient.CreateNewAccountAsync(ContactEmail);

            // Create new Order
            var order = await acmeClient.NewOrderAsync(account, Identifiers);

            // Create DNS challenge (DNS is required for wildcard certificate)
            var challenges = await acmeClient.GetDnsChallenges(account, order);

            // Creation of all DNS entries
            foreach (var challenge in challenges)
            {
                var dnsKey = challenge.VerificationKey;
                var dnsText = challenge.VerificationValue;
                // value can be e.g.: eBAdFvukOz4Qq8nIVFPmNrMKPNlO8D1cr9bl8VFFsJM

                // Create DNS TXT record e.g.:
                // key: _acme-challenge.your.domain.com
                // value: eBAdFvukOz4Qq8nIVFPmNrMKPNlO8D1cr9bl8VFFsJM
            }

            // Validation of all DNS entries
            foreach (var challenge in challenges)
            {
                await acmeClient.ValidateChallengeAsync(account, challenge);

                var freshChallenge = await acmeClient.GetChallengeAsync(account, challenge);
            }

            Assert.Pass();
        }
    }
}