using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs;

namespace Jhu.Graywulf.Jobs.CmdLineUtil
{
    [Verb(Name = "Test", Description = "Executes a test job.")]
    class Test : Parameters
    {
        public Test()
            : base()
        {
        }

        public override void Run()
        {
            var par = new Dictionary<string, object>();

            par["TestMethod"] = "AtomicDelay";

            RunWorkflow(typeof(Jhu.Graywulf.Scheduler.Jobs.Test.TestJob), par);
        }

#if false
        public override void Run()
        {
            ScheduleTestJob();
        }

        private void ScheduleTestJob()
        {
            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var ef = new EntityFactory(context);
                
                var f = ef.LoadEntity<Federation>(Federation.AppSettings.FederationName);
                var jd = ef.LoadEntity<JobDefinition>(String.Format("{0}.{1}", Cluster.AppSettings.ClusterName, typeof(Jhu.Graywulf.Jobs.Test.TestJob).Name));

                var queuename = String.Format("{0}.{1}", f.ControllerMachine.GetFullyQualifiedName(), Constants.MaintenanceQueueName);

                var job = jd.CreateJobInstance(queuename, ScheduleType.Queued);

                job.Save();
            }
        }
#endif
    }
}
