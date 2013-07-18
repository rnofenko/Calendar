using System.ComponentModel.DataAnnotations;

namespace Bs.Calendar.Models
{
    public class Room : Bases.BaseEntity
    {
        [StringLength(LENGTH_NAME)]
        public string Name { get; set; }
        public int NumberOfPlaces { get; set; }
        public int Color { get; set; }
    }
}
