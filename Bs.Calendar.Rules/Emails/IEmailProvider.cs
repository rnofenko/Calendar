namespace Bs.Calendar.Rules.Emails
{
    public interface IEmailProvider
    {
        EmailData Send(EmailData email);
    }
}
