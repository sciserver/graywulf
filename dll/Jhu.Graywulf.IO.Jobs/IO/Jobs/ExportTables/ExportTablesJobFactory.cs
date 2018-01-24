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
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.IO.Jobs.ExportTables
{
    [Serializable]
    public class ExportTablesJobFactory : JobFactoryBase
    {
        #region Static members

        public static ExportTablesJobFactory Create(Federation federation)
        {
            Type type = null;

            if (federation.ExportTablesJobFactory != null)
            {
                type = Type.GetType(federation.ExportTablesJobFactory);
            }

            // Fall back logic if config is invalid
            if (type == null)
            {
                type = typeof(ExportTablesJobFactory);
            }

            var factory = (ExportTablesJobFactory)Activator.CreateInstance(type, true);
            factory.RegistryContext = federation.RegistryContext;

            return factory;
        }

        #endregion
        #region Constructors and initializers

        protected ExportTablesJobFactory()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        protected ExportTablesJobFactory(RegistryContext context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        #endregion

        public virtual IEnumerable<ExportTablesMethod> EnumerateMethods()
        {
            yield return new ExportTablesToUriMethod();
        }

        public ExportTablesMethod GetMethod(string id)
        {
            return EnumerateMethods().First(i => i.ID == id);
        }

        public virtual ExportTablesParameters CreateParameters(Federation federation, Uri uri, Credentials credentials, SourceTableQuery source, string mimeType)
        {
            var sf = StreamFactory.Create(federation.StreamFactory);
            var ff = FileFormatFactory.Create(federation.FileFormatFactory);

            // TODO: this only supports single file exports

            // Check if output file is an archive
            var archival = sf.GetArchivalMethod(uri);
            var destination = ff.CreateFileFromMimeType(mimeType);

            if (archival == DataFileArchival.None)
            {
                // This is a single file export
                var compression = sf.GetCompressionMethod(uri);
                destination.Compression = compression;
                destination.Uri = uri;
                destination.Credentials = credentials;
            }
            else
            {
                destination.Compression = DataFileCompression.None;
                destination.Uri = new Uri(source.ObjectName + destination.Description.Extension, UriKind.RelativeOrAbsolute);
            }

            var export = new ExportTablesParameters()
            {
                Archival = archival,
                Sources = new [] { source },
                Destinations = new [] { destination },
                FileFormatFactoryType = federation.FileFormatFactory,
                StreamFactoryType = federation.StreamFactory,
            };

            if (archival == DataFileArchival.None)
            {
            }
            else if (archival == DataFileArchival.Zip)
            {
                export.Archival = archival;
                export.Uri = uri;
                export.Credentials = credentials;
            }
            else
            {
                throw new InvalidOperationException("Archival mode not supported for export.");
            }

            return export;
        }

        public JobInstance ScheduleAsJob(ExportTablesParameters parameters, string queueName, TimeSpan timeout, string comments)
        {
            JobInstance job = CreateJobInstance(null, GetJobDefinitionName(), queueName, timeout, comments);

            job.Parameters[Registry.Constants.JobParameterParameters].Value = parameters;

            return job;
        }

        private string GetJobDefinitionName()
        {
            return EntityFactory.CombineName(EntityType.JobDefinition, Registry.ContextManager.Configuration.FederationName, typeof(ExportTablesJob).Name);
        }

        public ExportTablesJobSettings GetJobDefinitionSettings()
        {
            var ef = new EntityFactory(RegistryContext);
            var jd = ef.LoadEntity<JobDefinition>(GetJobDefinitionName());
            return new ExportTablesJobSettings(jd.Settings);
        }
    }
}
