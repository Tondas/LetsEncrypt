using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LetsEncrypt.Client.Json
{
    public static class JsonSettings
    {
        public static JsonSerializerSettings CreateSettings()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
        }
    }
}