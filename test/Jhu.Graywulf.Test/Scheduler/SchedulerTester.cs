using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Scheduler
{
    public class SchedulerTester : ServiceTesterBase
    {
        public static SchedulerTester Instance
        {
            get { return CrossAppDomainSingleton<SchedulerTester>.Instance; }
        }

        public SchedulerTester()
        {
        }

        protected override void OnStart(object options)
        {
            Jhu.Graywulf.Scheduler.Program.StartDebug(options);
        }

        protected override void OnStop()
        {
            Jhu.Graywulf.Scheduler.Program.StopDebug();
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
                    Jhu.Graywulf.Scheduler.Program.DrainStopDebug();
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
                    Jhu.Graywulf.Scheduler.Program.KillDebug();
                    IsRunning = false;
                }
            }
        }
    }
}
