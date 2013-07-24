using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Bs.Calendar.Models;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class HomeBirthdaysVm
    {
        public HomeBirthdaysVm(User user)
        {
            if(user == null)
            {
                throw new ArgumentNullException("user instance reference cannot be null");
            }

            setup(user.Id, user.FirstName, user.LastName, user.Email, user.Role, user.Birthdate);
        }
        
        public HomeBirthdaysVm()
        {
        }

        private void setup(int id, string firstName, string lastName, string email, Roles role, DateTime birthdate)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
            Birthdate = birthdate;
        }

        public static implicit operator HomeBirthdaysVm(User user)
        {
            return new HomeBirthdaysVm(user);
        }

        public static implicit operator User(HomeBirthdaysVm user)
        {
            return new User()
                       {
                           Id = user.Id,
                           FirstName = user.FirstName,
                           LastName = user.LastName,
                           Role = user.Role,
                           Email = user.Email,
                           Birthdate = user.Birthdate
                       };
        }

        public int NewAge
        {
            get
            {
                DateTime currentDay = DateTime.Today;

                int newAge = currentDay.Year - Birthdate.Year;
                if (Birthdate > currentDay.AddYears(-newAge))
                {
                    --newAge;
                }

                return newAge;
            }
        }

        public int Id { get; set; }

        [StringLength(BaseEntity.LENGTH_NAME),
        Required,
        Display(Name = "First name")]
        public string FirstName { get; set; }

        [StringLength(BaseEntity.LENGTH_NAME),
        Required,
        Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required,
        EmailAddress,
        StringLength(BaseEntity.LENGTH_NAME)]
        public string Email { get; set; }

        public string UserLogin
        {
            get { return Email; }
        }

        public Roles Role { get; set; }
        public DateTime Birthdate { get; set; }
    }
}