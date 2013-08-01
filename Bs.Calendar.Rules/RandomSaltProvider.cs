using System;
using System.Security.Cryptography;

namespace Bs.Calendar.Rules
{
    public class RandomSaltProvider : ISaltProvider
    {
        public string GetSalt(int saltLength)
        {
            var rng = new RNGCryptoServiceProvider();
            var saltBytes = new byte[saltLength];
            rng.GetNonZeroBytes(saltBytes);
            return BitConverter.ToString(saltBytes).Replace("-", "");
        }
    }
}