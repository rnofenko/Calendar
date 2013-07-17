using System;
using System.Drawing;

using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    /// <summary>
    /// Data associated with view for Room model
    /// </summary>
    public class RoomEditVm
    {
#region fields & properties

        protected string _sName;

        /// <summary>
        /// Allows to set or get the name of the edited room
        /// </summary>
        public string Name
        {
            get { return _sName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Reference to the name string cannot be null");

                _sName = value;
            }
        }

        /// <summary>
        /// Allows to get or set number of places to be used in the room.
        /// </summary>
        public int     NumberOfPlaces;
        /// <summary>
        /// Allows to get or set the marking color for the room
        /// </summary>
        public Color    Color;

#endregion

#region conversions

        public static implicit operator Room(RoomEditVm revRoomViewModel)
        {
            return new Room()
                       {
                           Name             = revRoomViewModel.Name,
                           NumberOfPlaces   = revRoomViewModel.NumberOfPlaces,
                           Color            = revRoomViewModel.Color.ToArgb()
                       };
        }

        public static implicit operator RoomEditVm(Room rRoom)
        {
            return new RoomEditVm(
                rRoom.Name,
                rRoom.NumberOfPlaces,
                Color.FromArgb(rRoom.Color));
        }

#endregion

#region constructors

        public RoomEditVm(
            string  sName,
            int     iNumberOfPlaces,
            Color   cColor              = default(Color))
        {
            this.Name           = sName;
            this.NumberOfPlaces = iNumberOfPlaces;
            this.Color          = cColor;
        }

        public RoomEditVm()
            : this(string.Empty,0)
        {}

#endregion
    }
}