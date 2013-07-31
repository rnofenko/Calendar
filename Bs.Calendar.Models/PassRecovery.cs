using System;
using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class PassRecovery : BaseEntity
    {
        public string PasswordKeccakHash { get; set; }

        public DateTime Date { get; set; }
    }
}
