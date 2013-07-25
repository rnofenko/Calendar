using System.Net.Mail;
using System.Threading;
using Bs.Calendar.Rules;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    [TestFixture]
    class EmailTest
    {
        [Test]
        // this is interactive test. If it succed, you should check you email, if you got a lettert
        public void Can_send_test_email()
        {
            const string email = "d.ark.3.1415@gmail.com";  
            var message = new MailMessage();
            message.To.Add(email);
            message.Subject = "Test Mail";
            message.Body = "This is the message body";
            var sender = new EmailSender();
            sender.SendEmail(message);
            // Thread.Sleep(10000);    
        }
    }
}
