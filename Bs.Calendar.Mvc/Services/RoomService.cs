using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Models.Bases;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Mvc.ViewModels.Rooms;

namespace Bs.Calendar.Mvc.Services
{
    public class RoomService
    {
        private readonly RepoUnit _repoUnit;

        public RoomService(RepoUnit repository)
        {
            _repoUnit = repository;
        }

        public Room Get(int id)
        {
            var room = _repoUnit.Room.Get(id);
            return room;
        }

        public bool IsValid(RoomEditVm room)
        {
            bool result = true;
            result = result && (room.Name != string.Empty);
            result = result && (room.NumberOfPlaces > 0);
            result = result && (room.Color >= BaseEntity.MIN_COLOR_VALUE);
            result = result && (room.Color < BaseEntity.MAX_COLOR_VALUE);
            result = result && (
                !_repoUnit.Room.Load(r => r.Id != room.RoomId && r.Name == room.Name).Any());
            return result;
        }

        public RoomsVm Find(string searchStr)
        {
            var rooms = _repoUnit.Room.Load();

            if (!string.IsNullOrEmpty(searchStr))
            {
                searchStr = searchStr.ToLower();
                rooms = rooms.Where(
                    room => room.Name.ToLower().Contains(searchStr)
                    );
            }

            return new RoomsVm { Rooms = rooms.ToList() };
        }

        public void Delete(int id)
        {
            _repoUnit.Room.Delete(_repoUnit.Room.Get(id));
        }

        public void Save(RoomEditVm roomModel)
        {
            var room = Get(roomModel.RoomId) ?? new Room();
            room.Name = roomModel.Name;
            room.NumberOfPlaces = roomModel.NumberOfPlaces;
            room.Color = roomModel.Color;

            _repoUnit.Room.Save(room);
        }

        public IEnumerable<Room> GetAllRooms()
        {
            return _repoUnit.Room.Load().ToList();
        }
    }
}
