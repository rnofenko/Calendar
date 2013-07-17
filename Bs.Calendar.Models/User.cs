using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class User : BaseEntity
    {
        [StringLength(200)]
        public string Email { get; set; }

        public Roles Role { get; set; }
    }
}
