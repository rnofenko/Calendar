using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class UserEditVm
    {
        public UserEditVm(int userId,string firstName,string lastName, string email, Roles role)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
        }

        public UserEditVm()
        {
        }

        public int UserId { get; set; }

        [StringLength(50),
        Required,
        Display(Name = "First name")]
        public string FirstName { get; set; }

        [StringLength(50),
        Required,
        Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "An email is required"),
        EmailAddress,
        StringLength(200)]
        public string Email { get; set; }

        public string UserLogin
        {
            get { return Email; }
        }

        public Roles Role { get; set; }
    }
}