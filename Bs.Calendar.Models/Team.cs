using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class Team : BaseEntity
    {
        public int TeamID { get; set; }
        public string name { get; set; }
    }
}
