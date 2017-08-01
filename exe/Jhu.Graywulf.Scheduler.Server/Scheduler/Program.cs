using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;
using System.Configuration;
using System.Threading;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Scheduler
{
    public class Program
    {
        /// <summary>
        /// Program entry point
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Scheduler.Configuration.RunSanityCheck();

            // Switch between interactive (command-line) and service modes.
            if (Environment.UserInteractive ||
                (args != null && args.Length > 0 && args[0] == "-cmd"))
            {
                Console.WriteLine("Graywulf scheduler is starting...");

                // TODO: move to logger
#if BREAKDEBUG
                Console.WriteLine("Warning: built with BREAKDEBUG flag enabled.");
#endif

                QueueManager.Instance.Start(Jhu.Graywulf.Registry.ContextManager.Configuration.ClusterName, true);

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

        /// <summary>
        /// Stars the scheduler in debug mode, used for testing.
        /// </summary>
        internal static void StartDebug(object options)
        {
            if (options != null)
            {
                QueueManager.Instance.IsLayoutRequired = (bool)options;
            }
            
            QueueManager.Instance.Start(Jhu.Graywulf.Registry.ContextManager.Configuration.ClusterName, true);
        }

        /// <summary>
        /// Stops the scheduler when run in debug mode, used for testing.
        /// </summary>
        internal static void StopDebug()
        {
            QueueManager.Instance.Stop(TimeSpan.FromHours(1.5));
        }

        /// <summary>
        /// Drain-stops the scheduler when run in debug mode, used for testing.
        /// </summary>
        internal static void DrainStopDebug()
        {
            QueueManager.Instance.DrainStop(TimeSpan.FromMinutes(1.5));
        }

        /// <summary>
        /// Kills the scheduler when run in debug mode, used for testing.
        /// </summary>
        internal static void KillDebug()
        {
            QueueManager.Instance.Kill(TimeSpan.FromSeconds(20));
        }
    }
}
