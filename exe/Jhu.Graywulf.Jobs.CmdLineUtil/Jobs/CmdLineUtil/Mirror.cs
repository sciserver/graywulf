using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Jobs.CmdLineUtil
{
    [Verb(Name = "Mirror", Description = "Schedules a database mirroring job.")]
    class Mirror : Parameters
    {
        private string databaseVersionName;

        [Parameter(Name = "DatabaseVersion", Description = "Database version name.", Required = true)]
        public string DatabaseVersionName
        {
            get { return databaseVersionName; }
            set { databaseVersionName = value; }
        }

        public Mirror()
            :base()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.databaseVersionName = null;
            this.databaseVersionName = null;
        }

        public override void Run()
        {
            var par = new Dictionary<string, object>();
            par["DatabaseVersionName"] = databaseVersionName;

            RunWorkflow(typeof(Jhu.Graywulf.Registry.Jobs.MirrorDatabase.MirrorDatabaseJob), par);
        }

        /*public override void Run()
        {
            base.Run();

            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var ef = new EntityFactory(context);

                var c = ef.LoadEntity<Cluster>(Cluster.AppSettings.ClusterName);
                SignIn(c);

                var f = ef.LoadEntity<Federation>(Federation.AppSettings.FederationName);
                var jd = ef.LoadEntity<JobDefinition>(String.Format("{0}.{1}", Jhu.Graywulf.Registry.Cluster.AppSettings.ClusterName, typeof(Jobs.MirrorDatabase.MirrorDatabaseJob).Name));

                string queuename = String.Format("{0}.{1}", f.ControllerMachine.GetFullyQualifiedName(), Constants.MaintenanceQueueName);

                var job = jd.CreateJobInstance(queuename, ScheduleType.Queued);

                job.Parameters["DatabaseDefinitionName"].SetValue(databaseDefinitionName);
                job.Parameters["RedundancyStateName"].SetValue(databaseVersionName);
                
                job.Save();
            }
        }*/
    }
}
