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
        /// Creates parameters for a single file or single-archive import job
        /// </summary>
        public ImportTablesParameters CreateParameters(Federation federation, Uri uri, DestinationTable destination)
        {
            var sf = StreamFactory.Create(federation.StreamFactory);

            // Check if input file is an archive
            var archival = sf.GetArchivalMethod(uri);

            if (archival == DataFileArchival.None)
            {
                // This is a single file import
                // In single file mode the source file and destination must
                // be set explicitly

                var ff = FileFormatFactory.Create(federation.FileFormatFactory);
                var source = ff.CreateFile(uri);

                return new ImportTablesParameters()
                {
                    Sources = new [] { source },
                    Destinations = new[] { destination },
                    Archival = DataFileArchival.None,
                    Uri = uri,
                    FileFormatFactoryType = federation.FileFormatFactory,
                    StreamFactoryType = federation.StreamFactory,
                };
            }
            else
            {
                // This is an import of an entire archive
                // In archive mode, source file formats are figured out automatically
                // and destination is a table name pattern only

                return new ImportTablesParameters()
                {
                    Sources = null, 
                    Destinations = new[] { destination },
                    Archival = archival,
                    Uri = uri,
                    FileFormatFactoryType = federation.FileFormatFactory,
                    StreamFactoryType = federation.StreamFactory,
                };
            }
        }

        public JobInstance ScheduleAsJob(ImportTablesParameters parameters, string queueName, string comments)
        {
            JobInstance job = CreateJobInstance(null, GetJobDefinitionName(), queueName, comments);

            job.Parameters[Constants.JobParameterImport].Value = parameters;

            return job;
        }

        private string GetJobDefinitionName()
        {
            return EntityFactory.CombineName(EntityType.JobDefinition, Registry.ContextManager.Configuration.FederationName, typeof(ImportTablesJob).Name);
        }

        public ImportTablesJobSettings GetJobDefinitionSettings()
        {
            var ef = new EntityFactory(Context);
            var jd = ef.LoadEntity<JobDefinition>(GetJobDefinitionName());
            return new ImportTablesJobSettings(jd.Settings);
        }
    }
}
