using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class User : BaseEntity
    {
        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(20)]
        public string FirstName { get; set; }

        [StringLength(20)]
        public string LastName { get; set; }

        public string UserLogin
        {
            get { return Email; }
        }

        public Roles Role { get; set; }
    }
}
