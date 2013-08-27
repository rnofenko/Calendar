using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class RoomEditVm
    {
        public RoomEditVm()
        {
        }

        public RoomEditVm(Room room)
        {
            RoomId = room.Id;
            Name = room.Name;
            NumberOfPlaces = room.NumberOfPlaces;
            Color = room.Color;
        }

        public Room Map()
        {
            return new Room
                       {
                           Id = RoomId,
                           Color = Color,
                           Name = Name,
                           NumberOfPlaces = NumberOfPlaces
                       };
        }

        public int RoomId { get; set; }

        [Display(Name = "Name"),
        StringLength(BaseEntity.LENGTH_NAME),
        Required(ErrorMessage = "The name of the room must be specified")]
        public string Name { get; set; }

        [Display(Name = "Number of places"),
        Required(ErrorMessage = "Number of places should be specified"),
        Range(1, int.MaxValue, ErrorMessage = "Value is out of allowed range")]
        public int NumberOfPlaces { get; set; }

        [Display(Name = "Color"),
        Required(ErrorMessage = "Color should be selected"),
        Range(BaseEntity.MIN_COLOR_VALUE, BaseEntity.MAX_COLOR_VALUE, ErrorMessage = "This color is not available")]
        public int Color { get; set; }
    }
}