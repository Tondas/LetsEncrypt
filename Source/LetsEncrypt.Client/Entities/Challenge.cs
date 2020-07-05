using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace LetsEncrypt.Client.Entities
{
    public class Challenge : BaseEntity
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("status")]
        public ChallengeStatus? Status { get; set; }

        [JsonProperty("validated")]
        public DateTime? Validated { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        //

        [JsonIgnore]
        public string DnsKey { get; set; }

        [JsonIgnore]
        public string VerificationKey { get; set; }

        [JsonIgnore]
        public string VerificationValue { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ChallengeStatus
    {
        [JsonProperty("pending")]
        Pending,

        [JsonProperty("processing")]
        Processing,

        [JsonProperty("valid")]
        Valid,

        [JsonProperty("invalid")]
        Invalid,
    }

    public static class ChallengeType
    {
        public const string Http01 = "http-01";

        public const string Dns01 = "dns-01";

        public const string TlsAlpn01 = "tls-alpn-01";
    }
}