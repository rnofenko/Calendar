using System;
using System.Threading;
using System.Threading.Tasks;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Rules.Logs
{
    public static class Logger
    {
        private static readonly RepoUnit _repoUnit = Ioc.Resolve<RepoUnit>();

        public static void Info(string message)
        {
            if (message == null)
            {
                return;
            }

            var logRecord = new CalendarLog
                                {
                                    LogType = LogTypes.Info,
                                    Message = message,
                                    Timestamp = Config.Instance.Now,
                                };

            saveRecord(logRecord);
        }

        public static void Warning(string message)
        {
            if (message == null)
            {
                return;
            }
            
            var logRecord = new CalendarLog
            {
                LogType = LogTypes.Warning,
                Message = message,
                Timestamp = Config.Instance.Now,
            };

            saveRecord(logRecord);
        }

        public static void Error(string message)
        {
            if (message == null)
            {
                return;
            }

            var logRecord = new CalendarLog
            {
                LogType = LogTypes.Error,
                Message = message,
                Timestamp = Config.Instance.Now,
            };

            saveRecord(logRecord);
        }

        public static void Error(string message, Exception exception)
        {
            if (exception == null || message == null)
            {
                return;
            }

            var logRecord = new CalendarLog
            {
                LogType = LogTypes.Error,
                Message = message,
                Timestamp = Config.Instance.Now,
                StackTrace = exception.StackTrace
            };

            saveRecord(logRecord);
        }

        public static void Error(Exception exception)
        {
            if (exception == null)
            {
                return;
            }

            var logRecord = new CalendarLog
            {
                LogType = LogTypes.Error,
                Message = exception.Message,
                Timestamp = Config.Instance.Now,
                StackTrace = exception.StackTrace
            };

            saveRecord(logRecord);
        }

        private async static void saveRecord(CalendarLog logEntity)
        {
            //await Task.Factory.StartNew(() => _repoUnit.CalendarLog.Save(logEntity));
        }
    }
}
