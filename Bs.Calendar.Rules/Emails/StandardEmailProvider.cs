using System.Net;
using System.Net.Mail;

namespace Bs.Calendar.Rules.Emails
{
    public class StandardEmailProvider : IEmailProvider
    {
        public void Send(MailMessage message)
        {
            if (message == null)
            {
                return;
            }
            message.From = new MailAddress("binary.calendar@gmail.com", "Binary Calendar");
            var smtp = new SmtpClient
                {
                    Port = 587,
                    Host = "smtp.gmail.com",
                    Credentials = new NetworkCredential("binary.calendar", "binarystudio"),
                };
            smtp.EnableSsl = true;
            smtp.Send(message);
        }
    }
}