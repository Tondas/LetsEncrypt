using LetsEncrypt.Core;
using LetsEncrypt.Core.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetsEncrypt.Test
{
    public class InitUT : BaseUT

    {
        [Test]
        public async Task Test()
        {
            // Create client alias core object
            var acmeClient = new AcmeClient();

            // Specify which environment you want to use
            await acmeClient.InitAsync(EnviromentUri);

            // Generate new RSA key pair or use existing
            acmeClient.GenerateKeyPair();

            // Create new Account
            var account = await acmeClient.NewAccountAsync(ContactEmail);

            // Create new Order
            var order = await acmeClient.NewOrderAsync(account, new List<string> { "suppo.biz", "*.suppo.biz" });

            // Create DNS challenge (DNS is required for wildcard certificate)
            var challanges = await acmeClient.GetDnsChallenges(account, order);

            // Creation of all DNS entries
            foreach (var challange in challanges)
            {
                var dnsKey = challange.VerificationKey;
                var dnsText = challange.VerificationValue;

                // Create DNS TXT record
                // key: _acme-challenge.turingion.com, value: dnsText
            }

            // Validation of all DNS entries
            foreach (var challange in challanges)
            {
                await acmeClient.ValidateChallengeAsync(account, challange);

                // Verify status of challenge
                var freshChallange = await acmeClient.GetChallengeAsync(account, challange);
                if (freshChallange.Status == ChallengeStatus.Invalid)
                {
                    throw new Exception("Something is wrong with your DNS TXT record(s)!");
                }
            }

            // Generate certificate
            var certificate = await acmeClient.GenerateCertificateAsync(account, order, "Suppo.biz", "SuperSecretPassword:D");

            // Save files locally
            await LocalFileHandler.WriteAsync("Suppo.biz.pfx", certificate.GeneratePfx());
            await LocalFileHandler.WriteAsync("Suppo.biz.crt", certificate.GenerateCrt());

            Assert.Pass();
        }
    }
}