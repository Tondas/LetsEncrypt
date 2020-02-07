using LetsEncrypt.Client;
using LetsEncrypt.Client.Entities;
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
            // Create client alias core object + specify which environment you want to use
            var acmeClient = new AcmeClient(EnviromentUri);

            // Create new Account
            var account = await acmeClient.CreateNewAccountAsync(ContactEmail);

            // Create new Order
            var order = await acmeClient.NewOrderAsync(account, new List<string> { "suppo.biz", "*.suppo.biz" });

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

                // Verify status of challenge
                var freshChallenge = await acmeClient.GetChallengeAsync(account, challenge);
                if (freshChallenge.Status == ChallengeStatus.Invalid)
                {
                    throw new Exception("Something is wrong with your DNS TXT record(s)!");
                }
            }

            // Generate certificate
            var certificate = await acmeClient.GenerateCertificateAsync(account, order, "Suppo.biz");

            // Save files locally
            var password = "tondastondasSuppo1+";
            await LocalFileHandler.WriteAsync("Suppo.biz.pfx", certificate.GeneratePfx(password));
            await LocalFileHandler.WriteAsync("Suppo.biz.crt", certificate.GenerateCrt(password));
            await LocalFileHandler.WriteAsync("Suppo.biz.crt.pem", certificate.GenerateCrtPem(password));
            await LocalFileHandler.WriteAsync("Suppo.biz.key.pem", certificate.GenerateKeyPem());

            //await acmeClient.RevokeCertificateAsync(certificate);

            Assert.Pass();
        }

        [Test]
        public async Task DummyUT()
        {
            Assert.Pass();
        }
    }
}