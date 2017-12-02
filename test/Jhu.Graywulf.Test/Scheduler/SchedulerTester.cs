using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Logging;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Scheduler
{
    public class SchedulerTester : ServiceTesterBase
    {
        private static QueueManager[] debugInstances;
        private static LoggingContext loggingContext;

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

            // Wait for the control service to come online
            // The role-based access control can take quite a few seconds for
            // the very first time a user is authenticated

            int q = 1;
            while (true)
            {
                try
                {
                    var control = ServiceHelper.CreateChannel<ISchedulerControl>(DnsHelper.Localhost, "Control", Scheduler.Configuration.Endpoint, TimeSpan.FromSeconds(q));
                    control.Hello();
                    break;
                }
                catch
                {
                    q *= 2;

                    if (q > 16)
                    {
                        throw;
                    }
                }
            }
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
            loggingContext = new LoggingContext(false);

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

            loggingContext.Pop();
        }

        /// <summary>
        /// Stops the scheduler when run in debug mode, used for testing.
        /// </summary>
        internal static void StopDebug()
        {
            loggingContext.Push();

            for (int i = 0; i < debugInstances.Length; i++)
            {
                debugInstances[i].Stop(TimeSpan.FromMinutes(2));
            }

            // Stop logger
            loggingContext.StopLogger();
            loggingContext.Dispose();
        }

        /// <summary>
        /// Drain-stops the scheduler when run in debug mode, used for testing.
        /// </summary>
        internal static void DrainStopDebug()
        {
            loggingContext.Push();

            for (int i = 0; i < debugInstances.Length; i++)
            {
                debugInstances[i].DrainStop(Constants.DrainStopTimeout);
            }

            // Stop logger
            loggingContext.StopLogger();
            loggingContext.Dispose();
        }

        /// <summary>
        /// Kills the scheduler when run in debug mode, used for testing.
        /// </summary>
        internal static void KillDebug()
        {
            loggingContext.Push();

            for (int i = 0; i < debugInstances.Length; i++)
            {
                debugInstances[i].Kill(TimeSpan.FromSeconds(30));
            }

            // Stop logger
            loggingContext.StopLogger();
            loggingContext.Dispose();
        }
    }
}
