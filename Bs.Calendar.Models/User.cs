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

#warning remove
        public string UserLogin
        {
            get { return Email; }
        }

        public Roles Role { get; set; }
    }
}
