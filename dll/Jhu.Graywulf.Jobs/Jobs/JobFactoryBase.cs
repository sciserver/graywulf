using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Jobs
{
    /// <summary>
    /// Classes derived from this class can be used to create jobs
    /// </summary>
    [Serializable]
    public abstract class JobFactoryBase : ContextObject
    {
        #region Static members

        [NonSerialized]
        private static object syncRoot;

        protected static object SyncRoot
        {
            get { return syncRoot; }
        }

        static JobFactoryBase()
        {
            syncRoot = new object();
        }

        #endregion
        #region Constructors and initializers

        public JobFactoryBase()
        {
        }

        public JobFactoryBase(Context context)
            : base(context)
        {
        }

        #endregion

        /// <summary>
        /// Returns an intialized JobInstance.
        /// </summary>
        /// <param name="jobDefinitionName"></param>
        /// <param name="queueName"></param>
        /// <param name="comments"></param>
        /// <returns></returns>
        /// <remarks>The parameters of the job are not initialized.</remarks>
        protected JobInstance CreateJobInstance(string jobName, string jobDefinitionName, string queueName, string comments)
        {
            var ef = new EntityFactory(Context);
            var jd = ef.LoadEntity<JobDefinition>(jobDefinitionName);

            var job = jd.CreateJobInstance(queueName, Jhu.Graywulf.Registry.ScheduleType.Queued);

            job.Name = String.IsNullOrWhiteSpace(jobName) ? JobInstanceFactory.GenerateUniqueJobID(Context) : jobName;
            job.Comments = comments;

            return job;
        }
    }
}
