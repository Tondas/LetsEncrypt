namespace LetsEncrypt.Client.Entities
{
    public class AccountPersisted
    {
        public string AccountContactEmail { get; set; }

        public string AccountLocation { get; set; }

        public string PrivateKeyPem { get; set; }

        public string PublicKeyPem { get; set; }
    }
}