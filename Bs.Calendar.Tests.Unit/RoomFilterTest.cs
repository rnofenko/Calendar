using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels.Rooms;
using Bs.Calendar.Rules;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FluentAssertions;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    class RoomFilterTest
    {
        private RoomService _roomService;
        private FakeConfig _config;

        private Dictionary<string, Room> _rooms = new Dictionary<string, Room>
                                        {
                                            {"Big conference hall", new Room { Color = 2, Name = "Big conference hall", NumberOfPlaces = 27 }},
                                            {"Small conference hall", new Room { Color = 3, Name = "Small conference hall", NumberOfPlaces = 11 }},
                                            {"Kitchen", new Room { Color = 5, Name = "Kitchen", NumberOfPlaces = 7 }},
                                            {"Basement", new Room { Color = 4, Name = "Basement", NumberOfPlaces = 23 }},
                                            {"Roof", new Room { Color = 1, Name = "Roof", NumberOfPlaces = 10 }},
                                        };

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            FakeDi.Register();

            _config = Config.Instance as FakeConfig;

            var repoUnit = new RepoUnit();
            Ioc.RegisterInstance<RepoUnit>(repoUnit);

            _rooms.Values.ToList().ForEach(repoUnit.Room.Save);

            _roomService = new RoomService(repoUnit);
            Ioc.RegisterInstance<RoomService>(_roomService);
        }

        [Test,
        TestCase(null),
        TestCase(""),
        TestCase(" "),
        TestCase("       ")]
        public void Should_return_all_rooms_When_filter_by_empty_string(string searchString)
        {
            //act
            var rooms = _roomService.RetreiveList(new RoomFilterVm {SearchString = searchString}).Rooms;

            //assert
            rooms.ShouldAllBeEquivalentTo(_rooms.Values);
        }

        [Test,
        TestCase("this name is absent")]
        public void Should_not_return_any_room_When_string_is_not_empty_and_there_is_no_room_with_name_containing_specified_string(string searchString)
        {
            //act
            var rooms = _roomService.RetreiveList(new RoomFilterVm { SearchString = searchString }).Rooms;

            //assert
            rooms.Should().BeEmpty();
        }

        [Test,
        TestCase("HaLl", new[] { "Big conference hall", "Small conference hall" }),
        TestCase("hall", new[] { "Big conference hall", "Small conference hall" }),
        TestCase("en", new[] { "Big conference hall", "Small conference hall", "Kitchen", "Basement" })]
        public void Should_return_rooms_which_names_contain_specified_substring_no_matter_what_its_case_is_When_filter_by_string(string searchString, string[] expectedRoomNames)
        {
            //arrange
            var expectedRooms = expectedRoomNames.Select(name => _rooms[name]);

            //act
            var rooms = _roomService.RetreiveList(new RoomFilterVm { SearchString = searchString }).Rooms;

            //assert
            rooms.ShouldAllBeEquivalentTo(expectedRooms);
        }

        [Test,
        TestCase("Name", new[] { "Basement", "Big conference hall", "Kitchen", "Roof", "Small conference hall" }),
        TestCase("Name DESC", new[] { "Small conference hall", "Roof", "Kitchen", "Big conference hall", "Basement" })]
        public void Should_sort_rooms_by_name(string sortByField, string[] expectedRoomNames)
        {
            //arrange
            var expectedRooms = expectedRoomNames.Select(name => _rooms[name]);

            //act
            var rooms = _roomService.RetreiveList(new RoomFilterVm { SortByField = sortByField }).Rooms;

            //assert
            rooms.ShouldAllBeEquivalentTo(expectedRooms);
        }

        [Test,
        TestCase("NumberOfPlaces DESC", new[] { 27, 23, 11, 10, 7 }),
        TestCase("NumberOfPlaces", new[] { 7, 10, 11, 23, 27 })]
        public void Should_sort_rooms_by_number_of_places(string sortByField, int[] expectedNumbers)
        {
            //act
            var rooms = _roomService.RetreiveList(new RoomFilterVm { SortByField = sortByField }).Rooms;

            //assert
            rooms.Select(room => room.NumberOfPlaces).ShouldBeEquivalentTo(expectedNumbers);
        }

        [Test,
        TestCase("Color DESC", new[] { 5, 4, 3, 2, 1 }),
        TestCase("Color", new[] { 1, 2, 3, 4, 5 })]
        public void Should_sort_rooms_by_colors(string sortByField, int[] expectedColors)
        {
            //act
            var rooms = _roomService.RetreiveList(new RoomFilterVm { SortByField = sortByField }).Rooms;

            //assert
            rooms.Select(room => room.Color).ShouldBeEquivalentTo(expectedColors);
        }
    }
}
