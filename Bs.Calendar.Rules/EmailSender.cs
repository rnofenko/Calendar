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
                Console.WriteLine("Thread starts");
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
                Console.WriteLine("Message is sent");
            }
        }

        public EmailSender()
        {
            Console.WriteLine(ThreadPool.SetMaxThreads(ThreadsCount, ThreadsCount));
        }

        public void SendEmail(MailMessage message)
        {
            var sender = new AsyncSender();
            Console.WriteLine("Ready");
            ThreadPool.QueueUserWorkItem(sender.Send, message);
            Console.WriteLine("Queued");
        }
    }
}
