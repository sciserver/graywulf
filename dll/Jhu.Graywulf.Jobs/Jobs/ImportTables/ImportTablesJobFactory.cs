using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.ImportTables
{
    [Serializable]
    public class ImportTablesJobFactory : JobFactoryBase
    {
        #region Static members

        public static ImportTablesJobFactory Create(Federation federation)
        {
            return new ImportTablesJobFactory(federation.Context);
        }

        #endregion

        protected ImportTablesJobFactory()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        protected ImportTablesJobFactory(Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        /// <summary>
        /// Creates parameters for a single-archive import job
        /// </summary>
        public ImportTablesParameters CreateParameters(Federation federation, Uri uri, DestinationTable destination, string queueName, string comments)
        {
            var ip = new ImportTablesParameters()
            {
                Sources = null, // in single archive mode, source file formats are figured out automatically
                Destinations = new [] {destination },   // this is a table name pattern only in this case
                Archival = DataFileArchival.Automatic,
                Uri = uri,
                FileFormatFactoryType = federation.FileFormatFactory,
                StreamFactoryType = federation.StreamFactory,
            };

            return ip;
        }

        public JobInstance ScheduleAsJob(ImportTablesParameters parameters, string queueName, string comments)
        {
            JobInstance job = CreateJobInstance(null, GetJobDefinitionName(), queueName, comments);

            job.Parameters[Constants.JobParameterImport].Value = parameters;

            return job;
        }

        private string GetJobDefinitionName()
        {
            return EntityFactory.CombineName(EntityType.JobDefinition, Registry.AppSettings.FederationName, typeof(ImportTablesJob).Name);
        }

        public ImportTablesJobSettings GetJobDefinitionSettings()
        {
            var ef = new EntityFactory(Context);
            var jd = ef.LoadEntity<JobDefinition>(GetJobDefinitionName());
            return new ImportTablesJobSettings(jd.Settings);
        }
    }
}
