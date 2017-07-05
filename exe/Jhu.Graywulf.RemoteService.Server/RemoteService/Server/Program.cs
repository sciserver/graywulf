using System;
using System.ServiceProcess;

namespace Jhu.Graywulf.RemoteService.Server
{
    public class Program
    {
        private static RemoteService service;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (Environment.UserInteractive)
            {
                // Run in command-prompt

                Start(args);

                Console.WriteLine("Press any key to stop service...");

                Console.ReadLine();

                Stop();
            }
            else
            {
                // Run as service

                ServiceBase.Run(new RemoteService());
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Util.ServiceHelper.WriteErrorDump(e);
        }

        public static void Start(string[] args)
        {
            service = new RemoteService();
            service.Start(args);
        }

        public static void Stop()
        {
            service.Stop();
            service = null;
        }
    }
}
