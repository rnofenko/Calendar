using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class User : BaseEntity, IPrincipal 
    {
        [StringLength(LENGTH_NAME)]
        public string Email { get; set; }

        [StringLength(LENGTH_NAME)]
        public string FirstName { get; set; }

        [StringLength(LENGTH_NAME)]
        public string LastName { get; set; }

        public Roles Role { get; set; }

        public string Password { get; set; }

        public bool IsInRole(string role)
        {
            throw new System.NotImplementedException();
        }

        public IIdentity Identity { get; set; }
    }
}
