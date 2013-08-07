using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bs.Calendar.Models;

namespace Bs.Calendar.Rules
{
    public class ContactTypeParser
    {
        public static ContactType GetContactType(string contact)
        {
            var contactType = ContactType.None;

            //contactType = isSkype(contact) ? ContactType.Skype : contactType;

            contactType = isUrl(contact) ? ContactType.Url : contactType;

            //contactType = isTwitter(contact) ? ContactType.Twitter : contactType;

            //contactType = isEmail(contact) ? ContactType.Email : contactType;

            //contactType = isPhone(contact) ? ContactType.Phone : contactType;
     
            return contactType;
        }

        private static bool isTwitter(string contact)
        {
            return Regex.IsMatch(contact, @"^@[a-zA-Z0-9_]{1,15}$");
        }

        private static bool isEmail(string contact)
        {
            return Regex.IsMatch(contact, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$");
        }

        private static bool isPhone(string contact)
        {
            return Regex.IsMatch(contact, @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$");
        }

        private static bool isSkype(string contact)
        {
            return Regex.IsMatch(contact, @"^[a-zA-Z][a-zA-Z0-9_\-\,\.]{5,31}$");
        }

        private static bool isUrl(string contact)
        {
            //return Regex.IsMatch(contact, @"(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?");
            return Regex.IsMatch(contact, @"([\w-]+\.)+");
        }

    }
}
