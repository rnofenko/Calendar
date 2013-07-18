using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class RoomEditVm
    {
        public int Id { get; set; }

        [StringLength(Models.Bases.BaseEntity.LENGTH_NAME),
        Required(ErrorMessage = "The name of the room must be specified")]
        public string Name { get; set; }

        public int NumberOfPlaces { get; set; }
        public Color Color { get; set; }

        public RoomEditVm(
            int id,
            string name,
            int numberOfPlaces,
            Color color = default(Color))
        {
            this.Id = id;
            this.Name = name;
            this.NumberOfPlaces = numberOfPlaces;
            this.Color = color;
        }

        public RoomEditVm()
            : this(0, string.Empty, 0)
        {}

        public static implicit operator Room(RoomEditVm roomViewModel)
        {
            if (roomViewModel == null)
                throw new ArgumentNullException("reference to the converted instance cannot be null");

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
            if(room == null)
                throw new ArgumentNullException("reference to the converted instance cannot be null");

            return new RoomEditVm(
                room.Id,
                room.Name,
                room.NumberOfPlaces,
                Color.FromArgb(room.Color));
        }
    }
}