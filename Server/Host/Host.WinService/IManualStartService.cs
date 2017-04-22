using System;

namespace Host.WinService
{
    /// <summary>
    /// Manual runner
    /// </summary>
    interface IManualStartService
    {
        /// <summary>
        /// Start
        /// </summary>
        /// <param name="args"></param>
        void ManualStart(String[] args);

        /// <summary>
        /// Stop
        /// </summary>
        void ManualStop();
    }

}
