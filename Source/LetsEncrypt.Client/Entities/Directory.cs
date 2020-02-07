using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LetsEncrypt.Client.Entities
{
    public class Directory : BaseEntity
    {
        [JsonProperty("newNonce")]
        public Uri NewNonce { get; set; }

        [JsonProperty("newAccount")]
        public Uri NewAccount { get; set; }

        [JsonProperty("newOrder")]
        public Uri NewOrder { get; set; }

        [JsonProperty("revokeCert")]
        public Uri RevokeCert { get; set; }

        [JsonProperty("keyChange")]
        public Uri KeyChange { get; set; }

        [JsonProperty("meta")]
        public DirectoryMeta Meta { get; set; }
    }

    public class DirectoryMeta
    {
        [JsonProperty("termsOfService")]
        public Uri TermsOfService { get; set; }

        [JsonProperty("website")]
        public Uri Website { get; set; }

        [JsonProperty("caaIdentities")]
        public List<string> CaaIdentities { get; set; }

        [JsonProperty("externalAccountRequired")]
        public bool? ExternalAccountRequired { get; set; }
    }
}