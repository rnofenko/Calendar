using System;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models.Bases;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Services
{
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

        public RoomEditVm CreateViewModel(RoomEditVm.RoomEditVmExtra extraInfo)
        {
            var roomViewModel = CreateViewModel();
            roomViewModel.Extra = extraInfo;

            return roomViewModel;
        }

        public void Save(RoomEditVm room)
        {
            _repoUnit.Room.Save(room);
        }

        public bool IsValid(RoomEditVm room)
        {
            return room.Name != string.Empty &&
                   room.NumberOfPlaces > 0 &&
                   room.Color >= BaseEntity.MIN_COLOR_VALUE &&
                   room.Color < BaseEntity.MAX_COLOR_VALUE;
        }

        public RoomEditVm Load(int id)
        {
            return _repoUnit.Room.Get(id) ?? null;
        }

        public void Delete(RoomEditVm room)
        {
            if (room == null)
            {
                throw new ArgumentNullException("reference to the deleted instance cannot be null");
            }

            _repoUnit.Room.Delete(room);
        }

        public void Delete(int id)
        {
            var room = _repoUnit.Room.Get(id);

            if(room == null)
            {
                throw new ArgumentNullException("reference to the deleted instance cannot be null");
            }

            _repoUnit.Room.Delete(room);
        }

        public RoomsVm Find(string searchStr)
        {
            var rooms = _repoUnit.Room.Load();

            return new RoomsVm() { Rooms = rooms.ToList() };
        }
    }
}
