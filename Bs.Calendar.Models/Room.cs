using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Models
{
    public class Room : BaseEntity
    {
        [StringLength(LENGTH_NAME)]
        public string Name { get; set; }

        public int NumberOfPlaces { get; set; }

        public int Color { get; set; }
    }
}
