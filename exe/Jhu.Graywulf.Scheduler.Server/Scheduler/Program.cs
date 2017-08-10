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
        private static QueueManager[] debugInstances;

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

                // Initialize logger
                LoggingContext.Current.StartLogger(Logging.EventSource.Scheduler, true);

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

                // Initialize logger
                LoggingContext.Current.StopLogger();
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
        internal static void StartDebug(SchedulerDebugOptions options)
        {
            // Initialize logger
            LoggingContext.Current.StartLogger(Logging.EventSource.Scheduler, true);

            if (options == null)
            {
                debugInstances = new QueueManager[1];
                debugInstances[0] = QueueManager.Instance;
                QueueManager.Instance.Start(Jhu.Graywulf.Registry.ContextManager.Configuration.ClusterName, true);
            }
            else
            {
                debugInstances = new QueueManager[options.InstanceCount];

                for (int i = 0; i < debugInstances.Length; i++)
                {
                    debugInstances[i] = new QueueManager();
                    debugInstances[i].IsControlServiceEnabled = false;
                    debugInstances[i].IsLayoutRequired = options.IsLayoutRequired;
                    debugInstances[i].Start(Registry.ContextManager.Configuration.ClusterName, true);
                }
            }
        }

        /// <summary>
        /// Stops the scheduler when run in debug mode, used for testing.
        /// </summary>
        internal static void StopDebug()
        {
            for (int i = 0; i < debugInstances.Length; i++)
            {
                debugInstances[i].Stop(TimeSpan.FromMinutes(2));
            }

            // Stop logger
            LoggingContext.Current.StopLogger();
        }

        /// <summary>
        /// Drain-stops the scheduler when run in debug mode, used for testing.
        /// </summary>
        internal static void DrainStopDebug()
        {
            for (int i = 0; i < debugInstances.Length; i++)
            {
                debugInstances[i].DrainStop(Constants.DrainStopTimeout);
            }

            // Stop logger
            LoggingContext.Current.StopLogger();
        }

        /// <summary>
        /// Kills the scheduler when run in debug mode, used for testing.
        /// </summary>
        internal static void KillDebug()
        {
            for (int i = 0; i < debugInstances.Length; i++)
            {
                debugInstances[i].Kill(TimeSpan.FromMinutes(2));
            }

            // Stop logger
            LoggingContext.Current.StopLogger();
        }
    }
}
