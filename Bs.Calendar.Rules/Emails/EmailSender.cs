using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Bs.Calendar.Rules.Logs;

namespace Bs.Calendar.Rules.Emails
{
    public class EmailSender
    {
        private readonly IEmailProvider _provider;

        public EmailSender(IEmailProvider provider)
        {
            _provider = provider;
        }

        public static bool IsValidEmailAddress(string emailAddress)
        {
            var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(emailAddress);
        }

        public void Send(string subject, string body, string addresser)
        {
            Send(subject, body, new List<string> {addresser});
        }

        public void Send(string subject, string body, IEnumerable<string> addressers)
        {
            if (!Config.Instance.SendEmail)
            {
                return;
            }

            foreach (var addresser in addressers)
            {
                if (!IsValidEmailAddress(addresser))
                {
                    continue;
                }

                sendAync(new EmailData { Addresser = addresser, Body = body, Subject = subject });
            }
        }

        private async void sendAync(EmailData email)
        {
            var task = Task<EmailData>.Factory.StartNew(() => _provider.Send(email));
            await task;

            if (task.Result.Result != null)
            {
                Logger.Error(task.Result.Result);
            }
        }
    }
}
