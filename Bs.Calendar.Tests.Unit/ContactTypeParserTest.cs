using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    public enum ContactType
    {
        None = 0,
        Email,
        Twitter,
        Skype,
        Phone,
        Url
    }

    [TestFixture]
    internal class ContactTypeParserTest
    {
        private ContactTypeParser _parser;

        [SetUp]
        public void Arrange()
        {
            _parser = new ContactTypeParser();
        }

        [TestCase("rnofenko@gmail.com", Result = ContactType.Email)]
        [TestCase("art.trubitsyn@gmail.com", Result = ContactType.Email)]
        public ContactType ContactParserShouldReturnEmailTypeIfInputStringIsEmail(string inputString)
        {
            // act && assert
            return _parser.GetContactType(inputString);
        }

        [TestCase("@rnofenko", Result = ContactType.Twitter)]
        [TestCase("@art713", Result = ContactType.Twitter)]
        public ContactType ContactParserShouldReturnTwitterTypeIfInputStringIsTwitterAccount(string inputString)
        {
            // act && assert
            return _parser.GetContactType(inputString);
        }

        [TestCase("r_nofenko", Result = ContactType.Skype)]
        [TestCase("art713", Result = ContactType.Skype)]
        public ContactType ContactParserShouldReturnSkypeTypeIfInputStringIsSkypeAccount(string inputString)
        {
            // act && assert
            return _parser.GetContactType(inputString);
        }

        [TestCase("+380502342671", Result = ContactType.Phone)]
        [TestCase("380502342671", Result = ContactType.Phone)]
        [TestCase("0502342671", Result = ContactType.Phone)]
        [TestCase("050-234-26-71", Result = ContactType.Phone)]
        [TestCase("050-23-42-671", Result = ContactType.Phone)]
        [TestCase("(050)2342671", Result = ContactType.Phone)]
        public ContactType ContactParserShouldReturnPhoneTypeIfInputStringIsPhoneNumber(string inputString)
        {
            // act && assert
            return _parser.GetContactType(inputString);
        }

        [TestCase("rnofenko.com", Result = ContactType.Url)]
        [TestCase("www.binary-studio.com", Result = ContactType.Url)]
        [TestCase("art713.gov.us", Result = ContactType.Url)]
        [TestCase("https://gmail.com", Result = ContactType.Url)]
        [TestCase("http://facebook.com", Result = ContactType.Url)]
        public ContactType ContactParserShouldReturnUrlTypeIfInputStringIsUrl(string inputString)
        {
            // act && assert
            return _parser.GetContactType(inputString);
        }

        public class ContactTypeParser
        {
            public ContactType GetContactType(string inputString)
            {
                return ContactType.None;
            }
        }
    }
}