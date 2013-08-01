using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;

namespace Jhu.Graywulf.RemoteService.Server
{
    public class Program
    {
        private static RemoteService service;

        static void Main(string[] args)
        {
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
