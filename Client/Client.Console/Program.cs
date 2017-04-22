using System;
using System.Runtime.InteropServices;

namespace ConsoleClient
{
    class Program
    {
        private static readonly WorkSession session = new WorkSession();

        private static HandlerRoutine _onClose = null;

        static void Main(string[] args)
        {
            // setup console handlers
            _onClose = new HandlerRoutine(ConsoleCtrlCheck);
            SetConsoleCtrlHandler(_onClose, true);

            // start worker
            session.Start();
            while (!session.Ended);
        }

        /// <summary>
        /// Handler for console close - gracefully close the session
        /// </summary>
        /// <param name="ctrlType"></param>
        /// <returns></returns>
        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            //Closing by X, Ctrl+C, Ctrl+Break
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                case CtrlTypes.CTRL_BREAK_EVENT:
                case CtrlTypes.CTRL_CLOSE_EVENT:
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    if (!session.Closed)
                        session.Quit();
                    Console.WriteLine("Program is being closed!");
                    break;

            }
            return true;
        }

        #region unmanaged

        /// <summary>
        /// System handler to catch console events
        /// </summary>
        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);

        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        #endregion
    }
}
