using System;

namespace Bs.Calendar.Rules.Logs
{
    public class Logger
    {
        public static void Error(string message)
        {
            try
            {
                //запись в базу или в файл.
            }
            catch
            {
                //совсем плохо.
            }
        }

        public static void Info(string message)
        {
        }

        public static void Error(string message, Exception exception)
        {
        }
    }
}
