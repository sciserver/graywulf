using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Jobs.CopyTables
{
    [Serializable]
    public class CopyTablesJobFactory : JobFactoryBase
    {
        #region Static members

        public static CopyTablesJobFactory Create(Context context)
        {
            return new CopyTablesJobFactory(context);
        }

        #endregion
        #region Constructors and initializer

        protected CopyTablesJobFactory()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        protected CopyTablesJobFactory(Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        #endregion

        public CopyTablesParameters CreateParameters()
        {
            return new CopyTablesParameters();
        }

        public JobInstance ScheduleAsJob(CopyTablesParameters parameters, string queueName, TimeSpan timeout, string comments)
        {
            var job = CreateJobInstance(null, GetJobDefinitionName(), queueName, timeout, comments);
            job.Parameters[Constants.JobParameterCopyTables].Value = parameters;
            return job;
        }

        private string GetJobDefinitionName()
        {
            return EntityFactory.CombineName(EntityType.JobDefinition, Context.Cluster.Name, Context.Domain.Name, Context.Federation.Name, typeof(CopyTablesJob).Name);
        }
    }
}
