namespace Bs.Calendar.Rules
{
    public class SimpleCryptoProvider : ICryptoProvider
    {
        public string GetHash(string data)
        {
            return data;
        }

        public string GetHashWithSalt(string data, string salt)
        {
            return data;
        }
    }
}
