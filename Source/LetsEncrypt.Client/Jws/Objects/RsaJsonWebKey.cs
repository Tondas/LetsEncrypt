using Newtonsoft.Json;

namespace LetsEncrypt.Client.Jws
{
    public class RsaJsonWebKey
    {
        [JsonProperty("e", Order = 1)]
        public string Exponent { get; set; }

        [JsonProperty("kty", Order = 2)]
        public string KeyType { get; set; }

        [JsonProperty("n", Order = 3)]
        public string Modulus { get; set; }
    }
}