using Bs.Calendar.Core;

namespace Bs.Calendar.Rules
{
    public class Config
    {
        private static readonly IConfig _config = Ioc.Resolve<IConfig>();

        public static IConfig Instance
        {
            get { return _config; }
        }
    }
}