using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
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
    class EmailEventReminderTest
    {
        private EmailEventReminder _emailEventReminder;
        private RepoUnit _repoUnit;
        private int _emailedUserCount;
        private Mock<EmailSender> _emailSenderMock;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            FakeDi.Register();
        }

        [SetUp]
        public void SetUp()
        {
            _emailedUserCount = 0;
            _emailSenderMock = new Mock<EmailSender>(Ioc.Resolve <IEmailProvider>());
            _emailSenderMock.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Callback<string, string, IEnumerable<string>>((subject, body, user) => _emailedUserCount += user.Count());

            _repoUnit = new RepoUnit();
            _emailEventReminder = new EmailEventReminder(_repoUnit, _emailSenderMock.Object);
        }

        [Test]
        public void EmailEventReminder_Should_Notify_Users_On_One_Event()
        {
            //arrange
            var calendarEvent = new CalendarEvent {DateStart = DateTime.Now, EventType = EventType.Personal, Id = 1};
            var testUsers = Builder<User>.CreateListOfSize(5).All().With(x => x.Live = LiveStatuses.Active).Build().ToList();
            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory {Event = calendarEvent, User = u}));

            //act
            _emailEventReminder.Start();

            //assert
            _emailSenderMock.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Exactly(1));
            _emailedUserCount.ShouldBeEquivalentTo(testUsers.Count);
        }

        [Test]
        public void EmailEventReminder_Should_Notify_Users_On_One_Multiple_Events() {
            //arrange
            var calendarEvent1 = new CalendarEvent { DateStart = DateTime.Now, EventType = EventType.Personal, Id = 1 };
            var calendarEvent2 = new CalendarEvent { DateStart = DateTime.Now, EventType = EventType.Personal, Id = 2 };
            var testUsers = Builder<User>.CreateListOfSize(5).All().With(x => x.Live = LiveStatuses.Active).Build().ToList();

            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory { Event = calendarEvent1, User = u }));
            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory { Event = calendarEvent2, User = u }));

            //act
            _emailEventReminder.Start();

            //assert
            _emailSenderMock.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Exactly(2));
            _emailedUserCount.ShouldBeEquivalentTo(testUsers.Count * 2);
        }

        [Test]
        public void EmailEventReminder_Should_Not_Notify_Users_On_Notified_Events() {
            //arrange
            var calendarEvent1 = new CalendarEvent { DateStart = DateTime.Now, EventType = EventType.Personal, Id = 1 };
            var calendarEvent2 = new CalendarEvent { DateStart = DateTime.Now, EventType = EventType.Personal, Id = 2 };
            var testUsers = Builder<User>.CreateListOfSize(5).All().With(x => x.Live = LiveStatuses.Active).Build().ToList();

            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory { Event = calendarEvent1, User = u }));
            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(
                new EmailOnEventHistory { Event = calendarEvent2, User = u, EmailOnEventStatus = EmailOnEventStatus.Sent}));

            //act
            _emailEventReminder.Start();

            //assert
            _emailSenderMock.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Exactly(1));
            _emailedUserCount.ShouldBeEquivalentTo(testUsers.Count);
        }

        [Test]
        public void EmailEventReminder_Should_Mark_Sent_Events_As_Notified() {
            //arrange
            var calendarEvent = new CalendarEvent { DateStart = DateTime.Now, EventType = EventType.Personal, Id = 1 };
            var testUsers = Builder<User>.CreateListOfSize(5).All().With(x => x.Live = LiveStatuses.Active).Build().ToList();

            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory
                .Save(new EmailOnEventHistory { Event = calendarEvent, User = u, EmailOnEventStatus = EmailOnEventStatus.NotSent }));

            //act
            _emailEventReminder.Start();

            //assert
            _repoUnit.EmailOnEventHistory
                .Load(h => h.EmailOnEventStatus == EmailOnEventStatus.NotSent)
                .Should().BeEmpty();
        }

        [Test]
        public void EmailEventReminder_Should_Send_Proper_Personal_Events() 
        {
            //arrange
            var calendarEvent1 = new CalendarEvent { DateStart = DateTime.Now, EventType = EventType.Personal, Id = 1 };
            var calendarEvent2 = new CalendarEvent { DateStart = DateTime.Now - new TimeSpan(24, 0, 0), EventType = EventType.Personal, Id = 2 };

            var testUsers = Builder<User>.CreateListOfSize(5).All().With(x => x.Live = LiveStatuses.Active).Build().ToList();

            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory { Event = calendarEvent1, User = u }));
            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory { Event = calendarEvent2, User = u }));

            //act
            _emailEventReminder.Start();

            //assert
            _emailSenderMock.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Exactly(2));
            _emailedUserCount.ShouldBeEquivalentTo(testUsers.Count * 2);
        }

        [Test]
        public void EmailEventReminder_Should_Send_Proper_Meeting_Events() 
        {
            //arrange
            var calendarEvent1 = new CalendarEvent { DateStart = DateTime.Now, EventType = EventType.Meeting, Id = 1 };
            var calendarEvent2 = new CalendarEvent { DateStart = DateTime.Now - new TimeSpan(24, 0, 0), EventType = EventType.Meeting, Id = 2 };

            var testUsers = Builder<User>.CreateListOfSize(5).All().With(x => x.Live = LiveStatuses.Active).Build().ToList();

            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory { Event = calendarEvent1, User = u }));
            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory { Event = calendarEvent2, User = u }));

            //act
            _emailEventReminder.Start();

            //assert
            _emailSenderMock.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Exactly(2));
            _emailedUserCount.ShouldBeEquivalentTo(testUsers.Count * 2);
        }

        [Test]
        public void EmailEventReminder_Should_Send_Proper_AllDay_Events() 
        {
            //arrange
            var calendarEvent1 = new CalendarEvent
            {
                DateStart = DateTime.Now, EventType = EventType.Meeting, Id = 1, IsAllDay = true
            };
            var calendarEvent2 = new CalendarEvent
            {
                DateStart = DateTime.Now - new TimeSpan(24, 0, 0), EventType = EventType.Meeting, Id = 2, IsAllDay = true
            };

            var testUsers = Builder<User>.CreateListOfSize(5).All().With(x => x.Live = LiveStatuses.Active).Build().ToList();

            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory { Event = calendarEvent1, User = u }));
            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory { Event = calendarEvent2, User = u }));

            //act
            _emailEventReminder.Start();

            //assert
            _emailSenderMock.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Exactly(2));
            _emailedUserCount.ShouldBeEquivalentTo(testUsers.Count * 2);
        }

        [Test]
        public void EmailEventReminder_Should_Not_Send_Improper_Personal_Events() {
            //arrange
            var properCalendarEvent = new CalendarEvent { DateStart = DateTime.Now, EventType = EventType.Personal, Id = 1 };
            var imProperCalendarEvent = new CalendarEvent { DateStart = DateTime.Now + new TimeSpan(4,0,0), EventType = EventType.Personal, Id = 2 };

            var testUsers = Builder<User>.CreateListOfSize(5).All().With(x => x.Live = LiveStatuses.Active).Build().ToList();

            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory { Event = properCalendarEvent, User = u }));
            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory { Event = imProperCalendarEvent, User = u }));

            //act
            _emailEventReminder.Start();

            //assert
            _emailSenderMock.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Exactly(1));
            _emailedUserCount.ShouldBeEquivalentTo(testUsers.Count);
        }

        [Test]
        public void EmailEventReminder_Should_Not_Send_Improper_Meeting_Events() {
            //arrange
            var properCalendarEvent = new CalendarEvent { DateStart = DateTime.Now, EventType = EventType.Meeting, Id = 1 };
            var imProperCalendarEvent = new CalendarEvent { DateStart = DateTime.Now + new TimeSpan(4, 0, 0), EventType = EventType.Meeting, Id = 2 };

            var testUsers = Builder<User>.CreateListOfSize(5).All().With(x => x.Live = LiveStatuses.Active).Build().ToList();

            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory { Event = properCalendarEvent, User = u }));
            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory { Event = imProperCalendarEvent, User = u }));

            //act
            _emailEventReminder.Start();

            //assert
            _emailSenderMock.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Exactly(1));
            _emailedUserCount.ShouldBeEquivalentTo(testUsers.Count);
        }

        [Test]
        public void EmailEventReminder_Should_Not_Send_Improper_AllDay_Events() {
            //arrange
            var properCalendarEvent = new CalendarEvent
            {
                DateStart = DateTime.Now + new TimeSpan(24,0,0), EventType = EventType.Personal, Id = 1, IsAllDay = true
            };
            var imProperCalendarEvent = new CalendarEvent
            {
                DateStart = DateTime.Now + new TimeSpan(48,0,0), EventType = EventType.Meeting, Id = 2, IsAllDay = true
            };

            var testUsers = Builder<User>.CreateListOfSize(5).All().With(x => x.Live = LiveStatuses.Active).Build().ToList();

            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory { Event = properCalendarEvent, User = u }));
            testUsers.ForEach(u => _repoUnit.EmailOnEventHistory.Save(new EmailOnEventHistory { Event = imProperCalendarEvent, User = u }));

            //act
            _emailEventReminder.Start();

            //assert
            _emailSenderMock.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()), Times.Exactly(1));
            _emailedUserCount.ShouldBeEquivalentTo(testUsers.Count);
        }
    }
}
