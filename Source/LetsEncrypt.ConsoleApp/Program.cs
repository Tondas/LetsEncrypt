using LetsEncrypt.Client;
using LetsEncrypt.Client.Entities;
using LetsEncrypt.Client.Interfaces;
using LetsEncrypt.Client.IO;
using LetsEncrypt.Client.Loggers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LetsEncrypt.ConsoleApp
{
    internal class Program
    {
        #region Fields + Properties

        private static Lazy<IServiceProvider> _serviceProvider = new Lazy<IServiceProvider>(InitDependencyInjection);
        private static IServiceProvider ServiceProvider => _serviceProvider.Value;

        private static ILogger Logger => ServiceProvider.GetRequiredService<ILogger>();
        private static LocalStorage LocalFileHandler => ServiceProvider.GetRequiredService<LocalStorage>();
        private static Settings Settings => ServiceProvider.GetRequiredService<Settings>();

        #endregion Fields + Properties

        //

        public static async Task Main(string[] args)
        {
            Console.WriteLine("--- LetsEncrypt.ConsoleApp ---");

            InitDependencyInjection();
            await Run();

            Console.WriteLine("Done.");
        }

        // Private Methods

        private static IServiceProvider InitDependencyInjection()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILogger, ConsoleLogger>();
            services.AddSingleton(typeof(LocalStorage));
            services.AddSingleton(typeof(Settings));

            return services.BuildServiceProvider();
        }

        private static async Task Run()
        {
            //
            Console.WriteLine("Step 1 - Order Creation");

            // Create client alias core object + specify which environment you want to use
            var acmeClient = new AcmeClient(ApiEnvironment.LetsEncryptV2);

            // Create new Account
            var account = await acmeClient.CreateNewAccountAsync(Settings.ContactEmail);

            // Create new Order
            var order = await acmeClient.NewOrderAsync(account, Settings.Domains);

            // Create DNS challenge (DNS is required for wildcard certificate)
            var challenges = await acmeClient.GetDnsChallenges(account, order);

            //
            Console.WriteLine("Step 1 - Done");
            Console.WriteLine("Step 2 - Verification by DNS challenge");

            // Creation of all DNS entries
            var sb = new StringBuilder(256);
            foreach (var challenge in challenges)
            {
                sb.AppendLine(string.Format("DNS TXT record Key: {0}", challenge.DnsKey));
                sb.AppendLine(string.Format("DNS TXT record Value: {0}", challenge.VerificationValue));
                sb.AppendLine();
            }
            await LocalFileHandler.WriteAsync("_Output.txt", sb.ToString());

            //
            Console.WriteLine("Step 2 - Open '_Output.txt' file and configure DNS TXT record(s)");
            Console.WriteLine("Step 2 - Press any key to continue ...");
            Console.Read();

            //
            Console.WriteLine("Step 2 - Done");
            Console.WriteLine("Step 3 - Verification of DNS TXT record(s)");

            // Validation of all DNS entries
            var failedCount = 3;
            var valid = false;
            while (!valid)
            {
                try
                {
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

                    valid = true;
                }
                catch (Exception ex)
                {
                    failedCount--;

                    if (failedCount == 0)
                    {
                        throw new Exception("Validation of DNS TXT record(s) is failed!", ex);
                    }

                    Thread.Sleep(5000);
                }
            }

            //
            Console.WriteLine("Step 3 - Done");
            Console.WriteLine("Step 4 - Certificate generation");

            Thread.Sleep(5000);

            // Generate certificate
            var certificate = await acmeClient.GenerateCertificateAsync(account, order, Settings.CertificateFileName);

            // Save files locally
            await LocalFileHandler.WriteAsync(Settings.CertificateFileName + ".pfx", certificate.GeneratePfx(Settings.CertificatePassword));
            await LocalFileHandler.WriteAsync(Settings.CertificateFileName + ".crt", certificate.GenerateCrt(Settings.CertificatePassword));
            await LocalFileHandler.WriteAsync(Settings.CertificateFileName + ".crt.pem", certificate.GenerateCrtPem(Settings.CertificatePassword));
            await LocalFileHandler.WriteAsync(Settings.CertificateFileName + ".key.pem", certificate.GenerateKeyPem());

            Console.WriteLine("Step 4 - Done");
        }
    }
}