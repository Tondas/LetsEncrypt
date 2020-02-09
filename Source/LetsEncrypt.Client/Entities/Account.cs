using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LetsEncrypt.Client.Entities
{
    public partial class Account : BaseEntity
    {
        [JsonProperty("status")]
        public AccountStatus? Status { get; set; }

        [JsonProperty("contact")]
        public List<string> Contact { get; set; }

        [JsonProperty("termsOfServiceAgreed")]
        public bool? TermsOfServiceAgreed { get; set; }

        [JsonProperty("initialIp")]
        public string InitialIp { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum AccountStatus
    {
        [EnumMember(Value = "valid")]
        Valid,

        [EnumMember(Value = "deactivated")]
        Deactivated,

        [EnumMember(Value = "revoked")]
        Revoked,
    }
}