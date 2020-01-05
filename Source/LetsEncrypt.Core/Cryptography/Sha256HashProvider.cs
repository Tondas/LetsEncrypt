using System.Security.Cryptography;

namespace LetsEncrypt.Core.Cryptography
{
    public class Sha256HashProvider
    {
        public static byte[] ComputeHash(byte[] data)
        {
            using (var hasher = new SHA256Managed())
            {
                return hasher.ComputeHash(data);
                //var hashBytes = hasher.ComputeHash(data);
                //return BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
            }
        }
    }
}