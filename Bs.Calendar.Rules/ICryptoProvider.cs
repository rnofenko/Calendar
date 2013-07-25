namespace Bs.Calendar.Rules
{
    public interface ICryptoProvider
    {
        string GetKeccakHash(string data);
        string GetKeccakHashWithSalt(string data);
    }
}