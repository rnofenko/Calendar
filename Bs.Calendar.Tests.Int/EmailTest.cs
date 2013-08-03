using Bs.Calendar.Rules.Emails;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    public class EmailTest
    {
        [Ignore]
        [Test]
        public void Can_send_test_email()
        {
            const string email = "d.ark.3.1415@gmail.com";  
            var sender = new EmailSender();
            sender.Send("Test subj", "body", email);
        }
    }
}