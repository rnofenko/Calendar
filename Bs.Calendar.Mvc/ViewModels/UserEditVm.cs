using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class UserEditVm
    {
        public UserEditVm(int userId,string firstName,string lastName, string email, Roles role, DateTime? bday, LiveState liveState)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
            BirthDate = bday;
            LiveState = liveState;
        }

        public UserEditVm(User user)
        {
            UserId = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Role = user.Role;
            BirthDate = user.BirthDate ?? DateTime.Today;
            LiveState = user.LiveState;
            Contacts = new List<Contact>(user.Contacts);
        }
        
        public UserEditVm()
        {
        }

        public int UserId { get; set; }

        [StringLength(50),
        Required(ErrorMessage = "First name is required!"),
        Display(Name = "First name")]
        public string FirstName { get; set; }

        [StringLength(50),
        Required(ErrorMessage = "Last name is required!"),
        Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "E-mail is required!"),
        StringLength(200)]
        public string Email { get; set; }

        [DataType(DataType.Date),
        Display(Name = "Birth date"),
        DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true),
        Required(ErrorMessage = "Birth date is required!")]
        public DateTime? BirthDate { get; set; }

        public string UserLogin
        {
            get { return Email; }
        }

        public Roles Role { get; set; }

        public LiveState LiveState { get; set; }

        public List<Contact> Contacts { get; set; } 
    }
}