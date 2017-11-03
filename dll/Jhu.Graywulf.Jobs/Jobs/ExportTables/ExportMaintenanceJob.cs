using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Serialization;
using System.IO;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Jobs.ExportTables
{
    public class ExportMaintenanceJob : JobCodeActivity, IJobActivity
    {
        /// <summary>
        /// Age (in days) of exported data to be deleted
        /// </summary>
        [RequiredArgument]
        public InArgument<int> Age { get; set; }

        protected override void OnExecute(CodeActivityContext activityContext, CancellationContext cancellationContext)
        {
            int age = Age.Get(activityContext);

            throw new NotImplementedException();

            /* TODO: implement
            using (Context context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                EntityFactory ef = new EntityFactory(context);
                JobDefinition jd = (JobDefinition)ef.LoadStronglyTypedEntity(String.Format("{0}.{1}", Federation.AppSettings.FederationName, typeof(ExportTableJob).Name));

                // *** TODO: update to handle multiple job types
                HashSet<Guid> jdguids = new HashSet<Guid>() { jd.Guid };

                JobInstanceFactory jf = new JobInstanceFactory(context);

                // Find all completed ExportTable jobs older than the specified time span
                List<JobInstance> jobs = new List<JobInstance>(
                    jf.FindJobInstances(context.UserGuid, Guid.Empty, jdguids, JobExecutionState.Completed | JobExecutionState.Failed).Where(jj => (DateTime.Now - jj.DateFinished).Days > age));

                // Now delete output files
                foreach (JobInstance j in jobs)
                {
                    j.LoadParameters();

                    var et = (ExportTable)j.Parameters["Parameters"].GetValue();

                    try
                    {
                        // Delete output file s
                        et.DeleteOutput();

                        // Delete the job
                        j.Delete();
                    }
                    catch (IOException)
                    {
                        // *** TODO: write a log message
                    }
                }
            }*/
        }
    }
}
