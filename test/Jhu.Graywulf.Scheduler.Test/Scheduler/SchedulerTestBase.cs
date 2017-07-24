using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Scheduler
{
    public abstract class SchedulerTestBase: TestClassBase
    {
        public static void ClassInitialize()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                StartLogger();
                PurgeTestJobs();
            }
        }

        public static void ClassCleanUp()
        {
            using (SchedulerTester.Instance.GetExclusiveToken())
            {
                if (SchedulerTester.Instance.IsRunning)
                {
                    SchedulerTester.Instance.DrainStop();
                }

                PurgeTestJobs();
                StopLogger();
            }
        }
    }
}
