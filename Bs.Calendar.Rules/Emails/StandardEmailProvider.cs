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
                var message = new MailMessage(new MailAddress("bondinis@gmail.com", "Binary Calendar"),
                                              new MailAddress(email.Addresser, email.Addresser))
                    {
                        Subject = email.Subject,
                        Body = email.Body
                    };

                var smtp = new SmtpClient
                    {
                        Credentials = new NetworkCredential("bondinis@gmail.com", "dfygbc360"),
                        Host = "smtp.gmail.com",
                        Port = 587,
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