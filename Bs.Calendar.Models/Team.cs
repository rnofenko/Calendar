using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class Team : BaseEntity
    {
        [StringLength(200)]
        public string Name { get; set; }
    }
}
