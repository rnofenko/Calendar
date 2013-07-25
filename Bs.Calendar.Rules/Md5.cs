using System;
using System.Security.Cryptography;

namespace Bs.Calendar.Rules
{
    public static class Md5
    {
        public static string GetMd5Hash(string password)
        {
            var md5 = MD5.Create();
            return BitConverter.ToString(md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password))).Replace("-", "");
        }
    }
}