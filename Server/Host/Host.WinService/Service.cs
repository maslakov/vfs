using System.ServiceProcess;

namespace Host.WinService
{
    internal class Service
    {
        private static void Main(string[] args)
        {
            StartHelper.Start(new ServiceBase[] {new VfsWinService()}, args);
        }
    }
}