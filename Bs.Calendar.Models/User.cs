using System;

using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class User : BaseEntity
    {
        [StringLength(LENGTH_NAME)]
        public string Email { get; set; }

        [StringLength(LENGTH_NAME)]
        public string FirstName { get; set; }

        [StringLength(LENGTH_NAME)]
        public string LastName { get; set; }

        public Roles Role { get; set; }

        [DataType(DataType.Date)]
        public DateTime Birthdate { get; set; }
    }
}
