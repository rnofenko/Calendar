using System;
using System.Net;
using System.Net.Mail;
using System.Threading;

namespace Bs.Calendar.Rules
{
    public class EmailSender
    {
        private const int ThreadsCount = 10;

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

        public void SendEmail(MailMessage message)
        {
            var sender = new AsyncSender();
            // Sync call for sender. It will be removed soon.
            sender.Send(message);
            // Async call for sender. It doesn't work now
            // ThreadPool.QueueUserWorkItem(sender.Send, message);
        }
    }
}
