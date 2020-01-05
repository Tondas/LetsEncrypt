using Newtonsoft.Json;

namespace LetsEncrypt.Core.Jws
{
    public class JwsData
    {
        [JsonProperty("protected")]
        public string Protected { get; set; }

        [JsonProperty("payload")]
        public string Payload { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }
    }
}