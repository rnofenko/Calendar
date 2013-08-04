using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class Contact : BaseEntity
    {
        public ContactType ContactType { get; set; }

        public string Value { get; set; }
    }
}
