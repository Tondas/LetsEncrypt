using System;

namespace LetsEncrypt.Client.Jws
{
    public static class JwsConvert
    {
        public static string ToBase64String(byte[] data)
        {
            var s = Convert.ToBase64String(data);
            s = s.Split('=')[0];
            s = s.Replace('+', '-');
            s = s.Replace('/', '_');
            return s;
        }

        public static byte[] FromBase64String(string data)
        {
            var s = data;
            s = s.Replace('-', '+');
            s = s.Replace('_', '/');
            switch (s.Length % 4)
            {
                case 0: break;
                case 2: s += "=="; break;
                case 3: s += "="; break;
                default:
                    throw new Exception("Base64 string is not valid!");
            }
            return Convert.FromBase64String(s);
        }
    }
}