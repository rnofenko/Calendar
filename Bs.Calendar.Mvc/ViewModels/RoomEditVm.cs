using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

using Bs.Calendar.Models;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class RoomEditVm
    {
        public class RoomEditVmExtra
        {
            public string ViewTitle { get; set; }
            public string CallAction { get; set; }
            public string CallController { get; set; }
        }

        /// <summary>
        /// Extra data used to setup view
        /// </summary>
        public RoomEditVmExtra Extra { get; set; }

        public int Id { get; set; }

        [StringLength(Models.Bases.BaseEntity.LENGTH_NAME),
        Required(ErrorMessage = "The name of the room must be specified"),
        Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Number of places"),
        Required(ErrorMessage = "This value should be specified"),
        Range(1, int.MaxValue, ErrorMessage = "Value is out of allowed range")]
        public int NumberOfPlaces { get; set; }

        [Display(Name = "Color"),
        Range(BaseEntity.MIN_COLOR_VALUE, BaseEntity.MAX_COLOR_VALUE, ErrorMessage = "This color is not available")]
        public int Color { get; set; }

        /// <summary>
        /// Utility method used to setup class fields.
        /// </summary>
        private void setup(int id, string name, int numberOfPlaces, int color)
        {
            this.Id = id;
            this.Name = name;
            this.NumberOfPlaces = numberOfPlaces;
            this.Color = color;

            Extra = new RoomEditVmExtra();
        }

        public RoomEditVm(Room room)
        {
            if (room == null)
            {
                throw new ArgumentNullException("Model instance cannot be set to null");
            }

            setup(room.Id, room.Name, room.NumberOfPlaces, room.Color);
        }

        public RoomEditVm(int id, string name, int numberOfPlaces, int color)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name cannot be null");
            }

            setup(id, name, 0, color);
        }

        public RoomEditVm()
        {
            setup(0, string.Empty, 0, BaseEntity.MIN_COLOR_VALUE);
        }

        public static implicit operator Room(RoomEditVm roomViewModel)
        {
            if (roomViewModel == null)
            {
                throw new ArgumentNullException("reference to the converted instance cannot be null");
            }

            return new Room()
            {
                Id = roomViewModel.Id,
                Name = roomViewModel.Name,
                NumberOfPlaces = roomViewModel.NumberOfPlaces,
                Color = roomViewModel.Color
            };
        }

        public static implicit operator RoomEditVm(Room room)
        {
            return new RoomEditVm(room);
        }
    }
}