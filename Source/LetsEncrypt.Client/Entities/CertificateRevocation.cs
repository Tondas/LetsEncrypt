using Newtonsoft.Json;

namespace LetsEncrypt.Client.Entities
{
    public class CertificateRevocation
    {
        [JsonProperty("certificate")]
        public string Certificate { get; set; }

        [JsonProperty("reason")]
        public RevocationReason? Reason { get; set; }
    }

    public enum RevocationReason
    {
        Unspecified = 0,
        KeyCompromise = 1,
        CACompromise = 2,
        AffiliationChanged = 3,
        Superseded = 4,
        CessationOfOperation = 5,
        CertificateHold = 6,
        RemoveFromCRL = 8,
        PrivilegeWithdrawn = 9,
        AACompromise = 10,
    }
}