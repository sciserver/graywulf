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
            // Switch between interactive (command-line) and service modes.
            if (Environment.UserInteractive ||
                (args != null && args.Length > 0 && args[0] == "-cmd"))
            {
                RunInteractive();
            }
            else
            {
                // Run as service
                ServiceBase.Run(new SchedulerService());
            }
        }

        private static void RunInteractive()
        {
            using (var loggingContext = new LoggingContext(true, Components.AmbientContextStoreLocation.Default))
            {
                loggingContext.StartLogger(Logging.EventSource.Scheduler, true);

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

                loggingContext.StopLogger();
            }
        }
    }
}
