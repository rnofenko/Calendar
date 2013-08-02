namespace Bs.Calendar.Rules
{
    public interface ICryptoProvider
    {
        string GetHash(string data);
        string GetHashWithSalt(string data, string salt);
    }
}