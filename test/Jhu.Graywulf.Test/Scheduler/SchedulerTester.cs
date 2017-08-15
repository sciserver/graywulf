using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Logging;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Scheduler
{
    public class SchedulerTester : ServiceTesterBase
    {
        private static QueueManager[] debugInstances;

        public static SchedulerTester Instance
        {
            get { return CrossAppDomainSingleton<SchedulerTester>.Instance; }
        }

        public SchedulerTester()
        {
        }

        protected override void OnStart(object options)
        {
            StartDebug((SchedulerDebugOptions)options);
        }

        protected override void OnStop()
        {
            StopDebug();
        }

        public void DrainStop()
        {
            lock (SyncRoot)
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    DrainStopDebug();
                    IsRunning = false;
                }
            }
        }

        public void Kill()
        {
            lock (SyncRoot)
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    KillDebug();
                    IsRunning = false;
                }
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
                    if (i == 0)
                    {
                        debugInstances[i] = QueueManager.Instance;
                    }
                    else
                    {
                        debugInstances[i] = new QueueManager();
                    }

                    debugInstances[i].IsControlServiceEnabled = options.InstanceCount == 1 && options.IsControlServiceEnabled;
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
