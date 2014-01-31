using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Jobs.ExportTables
{
    [Serializable]
    public class ExportTablesFactory : JobFactoryBase
    {
        #region Static members

        public static ExportTablesFactory Create(Federation federation)
        {
            return new ExportTablesFactory(federation.Context);
        }

        #endregion

        protected ExportTablesFactory()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        protected ExportTablesFactory(Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        public JobInstance ScheduleAsJob(ExportTablesParameters parameters, string queueName, string comments)
        {
            JobInstance job = CreateJobInstance(
                EntityFactory.CombineName(EntityType.JobDefinition, Registry.AppSettings.FederationName, typeof(ExportTablesJob).Name),
                queueName,
                comments);

            job.Parameters[Constants.JobParameterExport].Value = parameters;

            return job;
        }
    }
}
