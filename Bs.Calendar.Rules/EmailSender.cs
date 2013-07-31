using System;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;

namespace Bs.Calendar.Rules
{
    public class EmailSender
    {
        private const int ThreadsCount = 10;

        public static bool IsValidEmailAddress(string emailAddress)
        {
            var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(emailAddress);
        }

        private class AsyncSender
        {            
            public void Send(object threadContext)
            {
                var message = threadContext as MailMessage;
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

        public EmailSender()
        {
            Console.WriteLine(ThreadPool.SetMaxThreads(ThreadsCount, ThreadsCount));
        }

        public virtual void SendEmail(MailMessage message)
        {
            if (!Config.Instance.SendEmail)
            {
                return;
            }

            var sender = new AsyncSender();
            // Sync call for sender. It will be removed soon.
            sender.Send(message);
            // Async call for sender. It doesn't work now
            // ThreadPool.QueueUserWorkItem(sender.Send, message);
        }
    }
}
