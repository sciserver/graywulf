using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Configuration;
using System.Threading;

namespace Jhu.Graywulf.Scheduler
{
    public class Program
    {
        //private static EventWaitHandle eh = new AutoResetEvent(false);   

        public static void Main(string[] args)
        {
            AppSettings.RunSanityCheck();

            if (Environment.UserInteractive)
            {
                Console.WriteLine("Graywulf scheduler is starting...");

                QueueManager.Instance.Start(Jhu.Graywulf.Registry.AppSettings.ClusterName, true);

                Console.WriteLine("                                  done.");

                Console.WriteLine("Graywulf scheduler is running...");
                Console.WriteLine("Press ENTER to shut down.");

                if (Console.ReadLine() == "kill")
                {
                    Console.WriteLine("Graywulf scheduler is canceling...");

                    QueueManager.Instance.Kill(TimeSpan.FromMinutes(5));
                }
                else
                {
                    Console.WriteLine("Graywulf scheduler is shutting down...");

                    QueueManager.Instance.Stop(TimeSpan.FromHours(1.5));
                }

                Console.WriteLine("                                       done.");
            }
            else
            {
                // Run as service
                ServiceBase.Run(new SchedulerService());
            }
        }

        public static void StartDebug()
        {
            AppSettings.RunSanityCheck();

            QueueManager.Instance.Start(Jhu.Graywulf.Registry.AppSettings.ClusterName, false);
        }

        public static void StopDebug()
        {
            QueueManager.Instance.Stop(TimeSpan.FromHours(1.5));
        }

        public static void DrainStopDebug()
        {
            QueueManager.Instance.DrainStop(TimeSpan.FromMinutes(1.5));
        }

        public static void KillDebug()
        {
            QueueManager.Instance.Kill(TimeSpan.FromSeconds(20));
        }
    }
}
