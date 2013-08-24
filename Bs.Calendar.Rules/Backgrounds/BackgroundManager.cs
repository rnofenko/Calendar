using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bs.Calendar.Core;
using System.Linq;
using Bs.Calendar.Rules.Logs;

namespace Bs.Calendar.Rules.Backgrounds
{
    public class BackgroundManager
    {
        private List<IBackgroundProcess> _processes;

        public void Start()
        {
            if (_processes == null || _processes.Count == 0)
            {
                _processes = Ioc.ResolveAll<IBackgroundProcess>().ToList();
            }

            Logger.Info(string.Format("There are {0} processes for run.", _processes.Count));

            var now = Config.Instance.Now;
            _processes
                .Where(x => x.GetNextTimeForStart() > now)
                .ToList()
                .ForEach(launch);
        }

        private void launch(IBackgroundProcess process)
        {
            Logger.Info(string.Format("Run process={0}.", process.GetType().FullName));

            try
            {
                new Task(process.Start).Start();
            }
            catch(Exception ex)
            {
                Logger.Error(string.Format("Executing process={0}.", process.GetType().FullName), ex);
            }
        }
    }
}