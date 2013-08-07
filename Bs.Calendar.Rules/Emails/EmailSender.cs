using System.Collections.Generic;
using System.Linq;
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

            var list = addressers
                .Where(IsValidEmailAddress)
                .Select(x => new EmailData {Addresser = x, Body = body, Subject = subject})
                .ToList();

            var thread = new Thread(sendInThread);
            thread.Start(list);
        }

        private void sendInThread(object list)
        {
            foreach (var email in (List<EmailData>)list)
            {
                sendAync(email);
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
