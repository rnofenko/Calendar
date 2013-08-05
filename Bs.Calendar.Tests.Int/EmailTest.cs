using System.Collections.Generic;
using Bs.Calendar.Core;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Rules;
using Bs.Calendar.Rules.Emails;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int
{
    public class EmailTest
    {
        [SetUp]
        public void SetUp()
        {
            DiMvc.Register();
        }

        [Ignore]
        [Test]
        public void Can_send_test_email()
        {
            const string EMAIL = "d.ark.3.1415@gmail.com";
            var sender = Ioc.Resolve<EmailSender>();
            sender.Send("Test subj", "body", EMAIL);
        }

        [Ignore]
        [Test]
        public void Can_send_test_email_many()
        {
            Config.Instance.SendEmail = true;

            var emails = new List<string>
                {
                    "rnofenko@gmail.com",
                    "rnofenko@gmail.com",
                    "rnofenko@gmail.com",
                    "rnofenko@gmail.com",
                    "rnofenko@gmail.com"
                };
            var sender = Ioc.Resolve<EmailSender>();
            sender.Send("Test subj", "body", emails);
        }
    }
}