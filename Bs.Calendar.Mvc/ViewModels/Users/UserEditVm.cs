﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels.Users
{
    public class UserEditVm
    {
        public UserEditVm(int userId, string firstName, string lastName, string email, Roles role, DateTime? bday,
                          LiveStatuses liveStatus = LiveStatuses.Active, ApproveStates approveState = ApproveStates.NotApproved)
        {
            UserId = userId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
            BirthDate = bday;

            Live = liveStatus;
            ApproveState = approveState;
        }

        public UserEditVm(User user)
        {
            UserId = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Role = user.Role;
            BirthDate = user.BirthDate;
            Contacts = new List<Contact>(user.Contacts);

            Live = user.Live;
            ApproveState = user.ApproveState;
        }
        
        public UserEditVm()
        {
        }

        public User Map()
        {
            return new User
            {
                Id = UserId,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                BirthDate = BirthDate,
                Contacts = Contacts,

                Role = Role,
                Live = Live,
                ApproveState = ApproveState,
            };
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
        DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? BirthDate { get; set; }

        public string UserLogin
        {
            get { return Email; }
        }

        public Roles Role { get; set; }

        public List<Contact> Contacts { get; set; }

        public LiveStatuses Live {get; set; }
        public ApproveStates ApproveState { get; set; }
    }
}