using System;

namespace Bs.Calendar.Rules.Backgrounds
{
    /// <summary>
    /// Logic is executed simultaneously in background.
    /// </summary>
    public interface IBackgroundProcess
    {
        /// <summary>
        /// Start executing logic.
        /// </summary>
        void Start();
        
        /// <summary>
        /// Returns time of next run.
        /// </summary>
        DateTime GetNextTimeForStart();
    }
}
