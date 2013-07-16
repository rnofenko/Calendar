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
        [StringLength(byte.MaxValue)]
        public string   Name;
        /// <summary>
        /// Total count of available places in the room
        /// </summary>
        public int      NumberOfPlaces;
        /// <summary>
        /// Unique color for room distinguishing
        /// </summary>
        public int      Color;
    }
}
