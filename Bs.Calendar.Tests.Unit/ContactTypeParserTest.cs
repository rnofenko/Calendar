using Bs.Calendar.Models;
using Bs.Calendar.Rules;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{

    [TestFixture]
    public class ContactTypeParserTest
    {
        [TestCase("rnofenko@gmail.com", Result = ContactType.Email)]
        [TestCase("art.trubitsyn@gmail.com", Result = ContactType.Email)]
        public ContactType ContactParserShouldReturnEmailTypeIfInputStringIsEmail(string inputString)
        {
            // act && assert
            return ContactTypeParser.GetContactType(inputString);
        }

        [TestCase("@rnofenko", Result = ContactType.Twitter)]
        [TestCase("@art713", Result = ContactType.Twitter)]
        public ContactType ContactParserShouldReturnTwitterTypeIfInputStringIsTwitterAccount(string inputString)
        {
            // act && assert
            return ContactTypeParser.GetContactType(inputString);
        }

        [TestCase("r_nofenko", Result = ContactType.Skype)]
        [TestCase("art713", Result = ContactType.Skype)]
        [TestCase("j*hn-nash", Result = ContactType.None)]
        public ContactType ContactParserShouldReturnSkypeTypeIfInputStringIsSkypeAccount(string inputString)
        {
            // act && assert
            return ContactTypeParser.GetContactType(inputString);
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
            return ContactTypeParser.GetContactType(inputString);
        }

        [TestCase("rnofenko.com", Result = ContactType.Url)]
        [TestCase("www.binary-studio.com", Result = ContactType.Url)]
        [TestCase("art713.gov.us", Result = ContactType.Url)]
        [TestCase("https://gmail.com", Result = ContactType.Url)]
        [TestCase("http://facebook.com", Result = ContactType.Url)]
        public ContactType ContactParserShouldReturnUrlTypeIfInputStringIsUrl(string inputString)
        {
            // act && assert
            return ContactTypeParser.GetContactType(inputString);
        }
    }
}