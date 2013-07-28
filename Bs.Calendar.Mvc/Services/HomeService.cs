using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.Services
{
    public class HomeService
    {
        public IEnumerable<User> LoadUsers()
        {
            using (var unit = new RepoUnit())
            {
                var users = unit.User.Load(u => ((DateTime)u.BirthDate).Month == DateTime.Now.Month && ((DateTime)u.BirthDate).Day >= DateTime.Now.Day && u.LiveState != LiveState.Deleted).ToList();

                //if (!users.Any())
                //{
                //    unit.User.Save(new User
                //    {
                //        Email = "rnofenko@gmail.com",
                //        Role = Roles.Admin,
                //        LiveState = LiveState.Active,
                //        FirstName = "Roman",
                //        LastName = "Nofenko",
                //        PasswordKeccakHash = "E9447A0B454AA39752445D6DCD2619F25C83F6453BA463C614820239CDC7CAB811F0C75D27776E119836523CF839C90596F2C0B07A45023741C200B51B6944D4",
                //        BirthDate = new DateTime(1991,7,27)
                //    });
                //    unit.User.Save(new User
                //    {
                //        Email = "art.trubitsyn@gmail.com",
                //        Role = Roles.Admin,
                //        LiveState = LiveState.Active,
                //        FirstName = "Artem",
                //        LastName = "Trubitsyn",
                //        PasswordKeccakHash = "4D14E3527467EA4AF5983EFF0C3FEECB48E7230EE7EAC0BA793B3B53B5BB6D1259E33C3ECA7B2A29AF4943391B913B8432F587D74D006D0AD9F3BFB3E8C4871F",
                //        BirthDate = new DateTime(1991,07,29)
                //    });
                //    users = unit.User.Load().ToList();
                //}              
                return users;
            }
        }
    }
}