using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services.Events;
using Bs.Calendar.Mvc.ViewModels.Events;
using Bs.Calendar.Rules;
using Bs.Calendar.Rules.Backgrounds;
using Bs.Calendar.Rules.Emails;
using Bs.Calendar.Tests.Unit.FakeObjects;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Unit
{
    [TestFixture]
    internal class EventSavingServiceTest
    {
        private EventSavingService _service;
        private RepoUnit _repoUnit;

        private Dictionary<string, User> _users = new Dictionary<string, User>
                                                      {
                                                          {"Friden", new User {Email = "user1@user1.com"}},
                                                          {"Stanne", new User {Email = "user2@user2.com"}},
                                                          {"Kokko", new User {Email = "user3@user3.com"}},
                                                      };

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            FakeDi.Register();
        }

        [SetUp]
        public void SetUp()
        {
            _repoUnit = new RepoUnit();
            Ioc.RegisterInstance<RepoUnit>(_repoUnit);

            _users.Values.ToList().ForEach(_repoUnit.User.Save);

            _service = new EventSavingService(_repoUnit);
            Ioc.RegisterInstance<EventSavingService>(_service);
        }

        #region Personal Event tests

        /*
         * Test coverage:
         * - personal event is being saved into the repository;
         * - service saves single PersonalEventLink record to the repository;
         * - service assigns correct user to the event;
         * - service assigns correct status for event;
         * - service saves correct event;
         */
        [Test,
        TestCase("Stanne")]
        public void Should_save_personal_event_for_existing_user(string userName)
        {
            //arrange
            var user = _repoUnit.User.Get(u => u.Email == _users[userName].Email);
            var personalEvent = new CalendarEventVm {EventType = EventType.Personal};

            var eventLink = new[] {new PersonalEventLink {EventStatus = 0, Event = personalEvent.Map(), User = user}};

            //act
            _service.Save(personalEvent, user.Id);

            //assert
            var events = _repoUnit.PersonalEvent.Load().ToList();
            events.ForEach(e => e.Id = 0); //To not consider record id's

            events.ShouldAllBeEquivalentTo(eventLink);
        }

        /*
         * Test coverage:
         * - email on event record is being saved into the repository;
         * - service saves single EmailOnEventHistory record to the repository;
         * - service assigns correct user to the event;
         * - service saves correct event;
         */
        [Test,
        TestCase("Stanne")]
        public void Should_save_email_on_event_history_When_save_personal_event_for_exisiting_user(string userName)
        {
            //arrange
            var user = _repoUnit.User.Get(u => u.Email == _users[userName].Email);
            var personalEvent = new CalendarEventVm { EventType = EventType.Personal };

            var mailingHistoryRecord = new[] { new EmailOnEventHistory { Event = personalEvent.Map(), User = user, EmailOnEventStatus = EmailOnEventStatus.NotSent } };

            //act
            _service.Save(personalEvent, user.Id);

            //assert
            var emailOnEventHistories = _repoUnit.EmailOnEventHistory.Load().ToList();
            emailOnEventHistories.ForEach(e => e.Id = 0); //To not consider record id's

            emailOnEventHistories.ShouldAllBeEquivalentTo(mailingHistoryRecord);
        }
        
        [Test]
        public void Should_not_save_personal_event_and_email_on_event_history_When_user_does_not_exist(string userName)
        {
            //arrange
            var notExistingUserId = 0;
            var personalEvent = new CalendarEventVm { EventType = EventType.Personal };

            //act
            _service.Save(personalEvent, notExistingUserId);

            //assert
            _repoUnit.EmailOnEventHistory.Load().Should().HaveCount(0);
            _repoUnit.EmailOnEventHistory.Load().Should().HaveCount(0);
        }

        [Test,
        TestCase("Stanne")]
        public void Should_not_create_any_record_in_team_event_repository_When_saving_personal_event(string userName)
        {
            //arrange
            var user = _repoUnit.User.Get(u => u.Email == _users[userName].Email);
            var personalEvent = new CalendarEventVm { EventType = EventType.Personal };

            var eventLink = new[] { new PersonalEventLink { EventStatus = 0, Event = personalEvent.Map(), User = user } };

            //act
            _service.Save(personalEvent, user.Id);

            //assert
            _repoUnit.TeamEvent.Load().Should().HaveCount(0);
        }

        #endregion
    }
}
