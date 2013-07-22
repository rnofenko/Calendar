using System;

namespace Bs.Calendar.Mvc.Services
{
    public class CryptoProvider
    {
        public string GetKeccakHash(string password)
        {
            var keccak512 = HashLib.HashFactory.Crypto.SHA3.CreateKeccak512();
            return keccak512.ComputeString(password).ToString();
        }

        public string GetSkeinHash(string password)
        {
            var skein512 = HashLib.HashFactory.Crypto.SHA3.CreateSkein512();
            return skein512.ComputeString(password).ToString();
        }
    }

    public static class Program
    {
        static void Main()
        {
            Console.WriteLine("HI!");
            var str = "";
            var crypto = new CryptoProvider();
            Console.WriteLine(crypto.GetKeccakHash(str));
            Console.ReadLine();
        }
    }
}