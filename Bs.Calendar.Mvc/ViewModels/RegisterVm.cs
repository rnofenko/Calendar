using System.ComponentModel.DataAnnotations;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class RegisterVm
    {
        [Required,
        EmailAddress]
        public string Email { get; set; }

        [Required,
        DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Password and confirmation should be the same")]
        public string ConfirmPassword { get; set; }
    }
}