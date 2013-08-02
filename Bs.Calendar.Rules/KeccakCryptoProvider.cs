using System;
using SHA3;

namespace Bs.Calendar.Rules
{
    public class KeccakCryptoProvider : ICryptoProvider
    {
        public string GetHash(string data)
        {
            var b = System.Text.Encoding.UTF8.GetBytes(data);
            var hash = new SHA3Managed(512).ComputeHash(b);
            return BitConverter.ToString(hash).Replace("-", "");
        }

        public string GetHashWithSalt(string data, string salt)
        {            
            return GetHash(String.Concat(data, salt));
        }
    }
}