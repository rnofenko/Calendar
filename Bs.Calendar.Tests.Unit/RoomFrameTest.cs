using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture] 
    class RoomFrameTest
    {
        private RoomService _roomService;
        private List<Room> _rooms;

        [TestFixtureSetUp]
        public void Setup()
        {
            FakeDi.Register();

            _rooms = new List<Room>
            {
                new Room {Name = "First room", Color = 1, NumberOfPlaces = 78},
                new Room {Name = "Second room", Color = 5, NumberOfPlaces = 12},
                new Room {Name = "Third room", Color = 2, NumberOfPlaces = 6},
            };

            var mockRepository = new Mock<IRoomRepository>();
            mockRepository.Setup(m => m.Load()).Returns(_rooms.AsQueryable());

            Ioc.RegisterInstance<IRoomRepository>(mockRepository.Object);
            _roomService = Ioc.Resolve<RoomService>();
        }

        
        [Test]
        [TestCase("Fi", 1)]
        [TestCase("s", 2)]
        [TestCase("fI", 1)]
        [TestCase("se", 1)]
        [TestCase("rOOm", 3)]
        [TestCase("rO1m", 0)]
        public void Find_Should_Return_Correct_Elements_Count_Test(string searchStr, int resultCount)
        {
            var rooms = _roomService.Find(searchStr).Rooms.ToList();
            rooms.Should().HaveCount(resultCount);
        }

        [Test]
        [TestCase("Fi", "First room")]
        [TestCase("sec", "Second room")]
        [TestCase("hIr", "Third room")]
        public void Find_Should_Return_Correct_Room(string searchStr, string roomName)
        {
            var rooms = _roomService.Find(searchStr).Rooms.ToList();
            rooms.Should().HaveCount(1);
            rooms[0].Name.Should().Be(roomName);
        }
    }
}
