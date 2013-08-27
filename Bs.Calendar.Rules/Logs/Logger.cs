using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Rules.Logs
{
    public class Logger
    {
        private static object _syncLock = new object();
        private static readonly RepoUnit _repoUnit = Ioc.Resolve<RepoUnit>();

        public static void Info(string message)
        {
            if (message != null)
            {
                saveRecord(new CalendarLog { LogType = LogTypes.Info, Message = message });
            }
        }

        public static void Warning(string message)
        {
            if (message != null)
            {
                saveRecord(new CalendarLog { LogType = LogTypes.Warning, Message = message });
            }
        }

        public static void Error(string message)
        {
            if (message != null)
            {
                saveRecord(new CalendarLog { LogType = LogTypes.Error, Message = message });
            }
        }

        public static void Error(string message, Exception exception)
        {
            if (exception != null && message != null)
            {
                saveRecord(new CalendarLog { LogType = LogTypes.Error, Message = message, StackTrace = exception.StackTrace });
            }
        }

        public static void Error(Exception exception)
        {
            if (exception != null)
            {
                saveRecord(new CalendarLog { LogType = LogTypes.Error, Message = exception.Message, StackTrace = exception.StackTrace });
            }
        }

        private static async void saveRecord(CalendarLog logEntity)
        {
            try { await prepareAndsaveRecordTask(logEntity); }
            catch (Exception exception) { Debug.WriteLine(exception); }
            finally
            {
                if (Monitor.IsEntered(_syncLock))
                {
                    Monitor.Exit(_syncLock);
                }
            }
        }

        private static async Task prepareAndsaveRecordTask(CalendarLog logEntity)
        {
            Monitor.Enter(_syncLock);

            logEntity.Timestamp = Config.Instance.Now;
            _repoUnit.CalendarLog.Save(logEntity);

            Monitor.Exit(_syncLock);
        }
    }
}
