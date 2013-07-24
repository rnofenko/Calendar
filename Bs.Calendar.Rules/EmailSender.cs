using System.Net;
using System.Net.Mail;

namespace Bs.Calendar.Rules
{
    public class EmailSender
    {
        public static void SendTestMail(string email)
        {
            var message = new MailMessage();
            message.To.Add(email);
            message.Subject = "Test Mail";
            message.Body = "This is the message body";
            EmailSender.SendEmail(message);            
        }

        public static void SendEmail(MailMessage message)
        {
            message.From = new MailAddress("binary.calendar@gmail.com", "Binary Calendar");
            var smtp = new SmtpClient
            {
                Port = 587,
                Host = "smtp.gmail.com",
                Credentials = new NetworkCredential("binary.calendar", "binarystudio"),
                EnableSsl = true
            };
            smtp.Send(message);
        }
    }
}
