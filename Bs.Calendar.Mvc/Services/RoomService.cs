using System;
using System.Linq;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Services
{
    /// <summary>
    /// Class used by the RoomController class for edit room management
    /// </summary>
    public class RoomService
    {
        private readonly RepoUnit _repoUnit;

        public RoomService(RepoUnit repository)
        {
            _repoUnit = repository;
        }

        public RoomEditVm CreateViewModel()
        {
            return new RoomEditVm();
        }

        public void Save(RoomEditVm room)
        {
            _repoUnit.Room.Save(room);
        }

        public bool IsValid(RoomEditVm room)
        {
            return room.Name != string.Empty &&
                   room.NumberOfPlaces > 0;
        }

        public RoomEditVm Load(int id)
        {
            return _repoUnit.Room.Get(id);
        }

        public void Delete(RoomEditVm room)
        {
            if(room == null)
            {
                throw new ArgumentNullException("reference to the deleted instance cannot be null");
            }

            _repoUnit.Room.Delete(room);
        }

        public void Delete(int id)
        {
            var room = _repoUnit.Room.Get(id);

            Delete(room);
        }

        public RoomsVm GetAllRooms()
        {
            var rooms = _repoUnit.Room.Load().ToList();

            return new RoomsVm { Rooms = rooms };
        }
    }
}
