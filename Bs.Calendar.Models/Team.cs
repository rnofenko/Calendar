using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class Team : BaseEntity
    {
        [StringLength(LENGTH_NAME)]
        public string Name { get; set; }

        [StringLength(LENGTH_NAME)]
        public string Description { get; set; }
    }
}
