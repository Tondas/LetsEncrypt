using Newtonsoft.Json;

namespace LetsEncrypt.Client.Jws
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