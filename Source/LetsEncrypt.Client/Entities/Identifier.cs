using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace LetsEncrypt.Client.Entities
{
    public class Identifier
    {
        [JsonProperty("type")]
        public IdentifierType Type { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IdentifierType
    {
        [EnumMember(Value = "dns")]
        Dns
    }
}