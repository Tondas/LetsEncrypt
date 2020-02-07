using Newtonsoft.Json;
using System;
using System.Net;

namespace LetsEncrypt.Client.Entities
{
    public class BaseEntity
    {
        [JsonIgnore]
        public virtual string UnknownContent { get; set; }

        [JsonIgnore]
        public virtual Uri Location { get; set; }

        [JsonIgnore]
        public virtual AcmeError Error { get; set; }
    }

    public class AcmeError
    {
        public string Type { get; set; }
        public string Detail { get; set; }
        public HttpStatusCode Status { get; set; }
    }
}