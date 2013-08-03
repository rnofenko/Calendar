using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using Bs.Calendar.Core;

namespace Bs.Calendar.Rules.Emails
{
    public class EmailSender
    {
        private readonly IEmailProvider _provider;

        public EmailSender(IEmailProvider provider)
        {
            _provider = provider;
        }

        private const int THREADS_COUNT = 10;

        public static bool IsValidEmailAddress(string emailAddress)
        {
            var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(emailAddress);
        }

        public EmailSender()
        {
            Console.WriteLine(ThreadPool.SetMaxThreads(THREADS_COUNT, THREADS_COUNT));
        }

        public void Send(string subject, string body, string addresser)
        {

        }

        public void Send(string subject, string body, IEnumerable<string> addressers)
        {
            if (!Config.Instance.SendEmail)
            {
                return;
            }

            
        }
    }
}
