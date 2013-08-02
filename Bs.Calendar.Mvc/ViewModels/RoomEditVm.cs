using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class RoomEditVm
    {
        public int RoomId { get; set; }

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

        [StringLength(BaseEntity.LENGTH_NAME),
        Required(ErrorMessage = "The name of the room must be specified"),
        Display(Name = "Name")]
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