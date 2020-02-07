using System;

namespace LetsEncrypt.Client.Entities
{
    public class ApiEnvironment
    {
        //// V1

        //public static Uri LetsEncryptV1 { get; } = new Uri("https://acme-v01.api.letsencrypt.org/directory");

        //public static Uri LetsEncryptV1Staging { get; } = new Uri("https://acme-staging.api.letsencrypt.org/directory");

        // V2

        public static Uri LetsEncryptV2 { get; } = new Uri("https://acme-v02.api.letsencrypt.org/directory");
        public static Uri LetsEncryptV2Staging { get; } = new Uri("https://acme-staging-v02.api.letsencrypt.org/directory");
    }
}