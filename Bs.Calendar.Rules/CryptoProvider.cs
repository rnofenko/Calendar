using System;
using System.Security.Cryptography;
using SHA3;

namespace Bs.Calendar.Rules
{
    public class CryptoProvider : ICryptoProvider
    {        
        public string GetKeccakHash(string password)
        {
            var b = System.Text.Encoding.UTF8.GetBytes(password);
            var hash = new SHA3Managed(512).ComputeHash(b);
            return BitConverter.ToString(hash).Replace("-", "");
        }

        public string GetKeccakHashWithSalt(string password)
        {
            var data = String.Concat(password, GetMd5Hash(password));
            return GetKeccakHash(data);
        }

        public string GetMd5Hash(string password)
        {
            var md5 = MD5.Create();
            return BitConverter.ToString(md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))).Replace("-", "");
        }
    }
}