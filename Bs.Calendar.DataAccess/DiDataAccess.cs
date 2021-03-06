﻿using Bs.Calendar.Core;

namespace Bs.Calendar.DataAccess
{
    public class DiDataAccess
    {
        public static void Register()
        {
            Ioc.RegisterType<IUserRepository, UserRepository>();
            Ioc.RegisterType<IRoomRepository, RoomRepository>();
            Ioc.RegisterType<ITeamRepository, TeamRepository>();
            Ioc.RegisterType<IBookRepository, BookRepository>();
            Ioc.RegisterType<ITagRepository, TagRepository>();
            Ioc.RegisterType<IBookHistoryRepository, BookHistoryRepository>();
            Ioc.RegisterType<IPersonalEventRepository, PersonalEventRepository>();
            Ioc.RegisterType<ITeamEventRepository, TeamEventRepository>();
            Ioc.RegisterType<ICalendarLogRepository, CalendarLogRepository>();

            Ioc.RegisterType<IEmailOnEventHistoryRepository, EmailOnEventHistoryRepository>();
        }
    }
}
