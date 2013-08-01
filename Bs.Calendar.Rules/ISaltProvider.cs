namespace Bs.Calendar.Rules
{
    public interface ISaltProvider
    {
        string GetSalt(int saltLength);
    }
}
