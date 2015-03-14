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
            Type type = null;

            if (federation.ImportTablesJobFactory != null)
            {
                type = Type.GetType(federation.ImportTablesJobFactory);
            }

            // Fall back logic if config is invalid
            if (type == null)
            {
                type = typeof(ImportTablesJobFactory);
            }

            var factory = (ImportTablesJobFactory)Activator.CreateInstance(type, true);
            factory.Context = federation.Context;

            return factory;
        }

        #endregion
        #region Constrcutors and initializers

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

        #endregion

        public virtual IEnumerable<ImportTablesMethod> EnumerateMethods()
        {
            yield return new ImportTablesFromUriMethod();
        }

        public ImportTablesMethod GetMethod(string id)
        {
            return EnumerateMethods().First(i => i.ID == id);
        }

        /// <summary>
        /// Creates parameters for a single file or single-archive import job
        /// </summary>
        public virtual ImportTablesParameters CreateParameters(Federation federation, Uri uri, Credentials credentials, DataFileBase source, DestinationTable destination)
        {
            var sf = StreamFactory.Create(federation.StreamFactory);
            var ff = FileFormatFactory.Create(federation.FileFormatFactory);

            // TODO: this only supports single file imports

            // Check if input file is an archive
            var archival = sf.GetArchivalMethod(uri);

            if (archival == DataFileArchival.None)
            {
                // This is a single file import
                // In single file mode the source file and destination must
                // be set explicitly

                // If source file is not passed as a parameter, create it automatically
                // from file extension
                if (source == null)
                {
                    source = ff.CreateFile(uri);
                }

                // For single file imports, credentials need to be set of the file level
                source.Credentials = credentials;

                return new ImportTablesParameters()
                {
                    Sources = new[] { source },
                    Destinations = new[] { destination },
                    Archival = DataFileArchival.None,
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
                    Sources = source == null ? null : new[] { source },
                    Destinations = new[] { destination },
                    Archival = archival,
                    Uri = uri,
                    Credentials = credentials,
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
