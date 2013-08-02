using System;

namespace Bs.Calendar.Rules
{
    class SimpleCryptoProvider : ICryptoProvider
    {
        public string GetHash(string data)
        {
            return data;
        }

        public string GetHashWithSalt(string data, string salt="")
        {
            return String.IsNullOrEmpty(salt) ? data : string.Format("{0}{1}", data, salt);
        }
    }
}
