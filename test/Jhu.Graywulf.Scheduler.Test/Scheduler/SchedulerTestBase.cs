using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Test;

namespace Jhu.Graywulf.Scheduler
{
    public abstract class SchedulerTestBase : TestClassBase
    {
        protected ServiceTesterToken token;

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

        public void TestInitialize(bool layout, int instances)
        {
            token = SchedulerTester.Instance.GetExclusiveToken();
            PurgeTestJobs();
            var options = new SchedulerDebugOptions()
            {
                IsControlServiceEnabled = true,
                IsLayoutRequired = layout,
                InstanceCount = instances
            };
            SchedulerTester.Instance.EnsureRunning(options);
        }

        public virtual void TestCleanup()
        {
            try
            {
                if (SchedulerTester.Instance.IsRunning)
                {
                    SchedulerTester.Instance.DrainStop();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                token.Dispose();
            }
        }
    }
}
