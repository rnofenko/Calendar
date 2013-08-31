using System;
using System.Threading;
using Bs.Calendar.Rules.Backgrounds;

namespace Bs.Calendar.Mvc.Server
{
    public static class BackgroundTimer
    {
        private static readonly Timer _timer = new Timer(onTimerElapsed);
        private static readonly BackgroundManager _manager = new BackgroundManager();

        public static void Start()
        {
            _timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(30000));
        }

        private static void onTimerElapsed(object sender)
        {
            _manager.Start();
        }
    }
}