using LetsEncrypt.Client.Json;
using Newtonsoft.Json;
using System;

namespace LetsEncrypt.Client
{
    public partial class AcmeClient : BaseAcmeClient
    {
        private readonly JsonSerializerSettings _jsonSettings = JsonSettings.CreateSettings();

        // Ctor

        public AcmeClient(Uri directoryUri)
            : base(directoryUri)
        {
        }
    }
}