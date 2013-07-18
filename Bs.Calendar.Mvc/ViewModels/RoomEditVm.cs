using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class RoomEditVm
    {
        public class RoomEditVmExtra
        {
            /// <summary>
            /// Title for the current working mode page
            /// </summary>
            public string ViewTitle { get; set; }
            public string CallAction { get; set; }
            public string CallController { get; set; }

            public RoomEditVmExtra()
            {
                ViewTitle = CallAction = CallController = string.Empty;
            }
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
        [Display(Name = "Number of places")]
        public int NumberOfPlaces { get; set; }
        [Display(Name = "Color")]
        public Color Color { get; set; }

        /// <summary>
        /// Utility method used to setup class fields.
        /// </summary>
        /// <remarks>
        /// Just want to keep constructor code clear but still unified and easy-testable
        /// </remarks>
        private void Setup(
            int id,
            string name,
            int numberOfPlaces,
            Color color)
        {
            this.Id = id;
            this.Name = name;
            this.NumberOfPlaces = numberOfPlaces;
            this.Color = color;

            Extra = new RoomEditVmExtra();
        }

        public RoomEditVm(Room room)
        {
            if(room == null)
            {
                throw new ArgumentNullException("Model instance cannot be set to null");
            }

            Setup(
                room.Id,
                room.Name,
                room.NumberOfPlaces,
                Color.FromArgb(room.Color));
        }

        public RoomEditVm(
            int id,
            string name,
            int numberOfPlaces,
            Color color)
        {
            if(name == null)
            {
                throw new ArgumentNullException("name cannot be null");
            }

            Setup(
                id,
                name,
                0,
                color);
        }

        public RoomEditVm()
        {
            Setup(
                0,
                string.Empty,
                0,
                Color.Black);
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
                           Color = roomViewModel.Color.ToArgb()
                       };
        }

        public static implicit operator RoomEditVm(Room room)
        {
            return new RoomEditVm(room);
        }
    }
}