using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Services
{
    public class ContactService
    {
        public List<Contact> UpdateContacts(List<Contact> contacts)
        {
            contacts = contacts ?? new List<Contact>();
            //contacts = contacts.Where(c => !string.IsNullOrEmpty(c.Value)).ToList();
            contacts.RemoveAll(c => string.IsNullOrEmpty(c.Value));

            if (contacts.Any(c => c.ContactType == ContactType.None && !string.IsNullOrEmpty(c.Value))) 
            {
                throw new WarningException(string.Format("Contact \"{0}\" is not recognizable",
                    contacts.First(c => c.ContactType == ContactType.None).Value));
            }

            return contacts;
        }
    }
}