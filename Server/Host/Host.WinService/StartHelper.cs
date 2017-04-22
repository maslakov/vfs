using System;
using System.ServiceProcess;

namespace Host.WinService
{
    /// <summary>
    /// Helper allows run an application both as windows service and console app
    /// </summary>
    public static class StartHelper
    {
        /// <summary>
        /// Replaces apps main, allows run service in console for debugging
        /// </summary>
        /// <param name="ServicesToRun">Services</param>
        /// <param name="args">parameters</param>
        public static void Start(ServiceBase[] ServicesToRun, String[] args)
        {
            bool manualStart = false;

            foreach (String cmd in args)
            {
                if (cmd.ToLower() == "/m")
                {
                    manualStart = true;
                    break;
                }
            }

            if (manualStart)
            {
                foreach (ServiceBase svc in ServicesToRun)
                {
                    if (svc is IManualStartService)
                    {
                        // Run service main
                        ((IManualStartService)svc).ManualStart(args);
                    }
                }

                Console.WriteLine("Press ENTER to finish...");
                Console.ReadLine();
            }
            else
            {
                ServiceBase.Run(ServicesToRun);
            }

            if (manualStart)
            {
                foreach (ServiceBase svc in ServicesToRun)
                {
                    if (svc is IManualStartService)
                    {
                        ((IManualStartService)svc).ManualStop();
                    }
                }
            }

        }
    }
}
