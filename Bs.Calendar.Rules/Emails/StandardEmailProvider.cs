using System;
using System.Net;
using System.Net.Mail;

namespace Bs.Calendar.Rules.Emails
{
    public class StandardEmailProvider : IEmailProvider
    {
        public EmailData Send(EmailData email)
        {
            if (email == null)
            {
                return new EmailData {Result = "Email class is NULL."};
            }

            try
            {
                var message = new MailMessage(new MailAddress("binary.calendar@gmail.com", "Binary Calendar"),
                                              new MailAddress(email.Addresser, email.Addresser))
                    {
                        Subject = email.Subject,
                        Body = email.Body
                    };

                var smtp = new SmtpClient
                    {
                        Port = 587,
                        Host = "smtp.gmail.com",
                        Credentials = new NetworkCredential("binary.calendar", "binarystudio"),
                        EnableSsl = true
                    };
                smtp.Send(message);
            }
            catch (Exception exception)
            {
                email.Result = exception.Message;
            }

            return email;
        }
    }
}