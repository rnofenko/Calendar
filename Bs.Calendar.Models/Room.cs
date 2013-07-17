using System.ComponentModel.DataAnnotations;

namespace Bs.Calendar.Models
{
    /// <summary>
    /// Entity class representing one room
    /// </summary>
    public class Room : Bases.BaseEntity
    {
        /// <summary>
        /// The name of the room
        /// </summary>
#warning put normal length
        [StringLength(byte.MaxValue)]
        public string Name { get; set; }

        /// <summary>
        /// Total count of available places in the room
        /// </summary>
        public int NumberOfPlaces { get; set; }

        /// <summary>
        /// Unique color for room distinguishing
        /// </summary>
        public int Color { get; set; }
    }
}
