using System;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class PasswordRecovery : BaseEntity
    {
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }

        public DateTime Date { get; set; }
    }
}
