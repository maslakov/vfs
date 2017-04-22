using System;
using System.ServiceModel;
using Core.Service;

namespace ConsoleHost
{
    /// <summary>
    /// Test console host
    /// </summary>
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                StartService();
                Console.WriteLine("Service is running...");
                Console.WriteLine("Press key to end...");
                Console.ReadLine();
                StopService();
            }
            catch (Exception ex)
            {
                Console.Write("The following exception was caught: " + Environment.NewLine + ex);
            }
            Console.ReadLine();
        }

        internal static ServiceHost myServiceHost = null;

        internal static void StartService()
        {
            myServiceHost = new ServiceHost(typeof(VfsService));
            myServiceHost.Open();
        }

        internal static void StopService()
        {
            if (myServiceHost.State != CommunicationState.Closed)
                myServiceHost.Close();
        }

    }
}
