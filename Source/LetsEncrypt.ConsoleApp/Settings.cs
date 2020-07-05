using System.Collections.Generic;
using System.Configuration;

namespace LetsEncrypt.ConsoleApp
{
    public class Settings
    {
        public string ContactEmail => GetValue(nameof(ContactEmail));

        public List<string> Domains
        {
            get
            {
                var domains = GetValue(nameof(Domains));
                var result = new List<string>();
                foreach (var item in domains.Split(','))
                {
                    result.Add(item.Trim());
                }

                return result;
            }
        }

        public string CertificateFileName => GetValue(nameof(CertificateFileName));
        public string CertificatePassword => GetValue(nameof(CertificatePassword));

        // Private Methods

        private string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}