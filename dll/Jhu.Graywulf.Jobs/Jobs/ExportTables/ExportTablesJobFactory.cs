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

namespace Jhu.Graywulf.Jobs.ExportTables
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
            factory.Context = federation.Context;

            return factory;
        }

        #endregion
        #region Constructors and initializers

        protected ExportTablesJobFactory()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        protected ExportTablesJobFactory(Context context)
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

        public virtual ExportTablesParameters CreateParameters(Federation federation, Uri uri, Credentials credentials, SourceTableQuery[] sources, string mimeType)
        {
            // Create destination files
            // One file per source

            var ff = FileFormatFactory.Create(federation.FileFormatFactory);

            var destinations = new DataFileBase[sources.Length];
            for (int i = 0; i < sources.Length; i++)
            {
                // TODO: use file extensions instead of type?
                var destination = ff.CreateFileFromMimeType(mimeType);
                destination.Uri = Util.UriConverter.FromFilePath(sources[i].SourceObjectName + destination.Description.Extension);

                // special initialization in case of a text file
                if (destination is TextDataFileBase)
                {
                    var tf = (TextDataFileBase)destination;
                    tf.Encoding = Encoding.ASCII;
                    tf.Culture = System.Globalization.CultureInfo.InvariantCulture;
                    tf.GenerateIdentityColumn = false;
                    tf.ColumnNamesInFirstLine = true;
                }

                destinations[i] = destination;
            }

            return new ExportTablesParameters()
            {
                Sources = sources,
                Destinations = destinations,
                Archival = DataFileArchival.Zip,        // TODO: change for single file exports
                Uri = uri,
                Credentials = credentials,
                FileFormatFactoryType = federation.FileFormatFactory,
                StreamFactoryType = federation.StreamFactory,
            };
        }
        
        public JobInstance ScheduleAsJob(ExportTablesParameters parameters, string queueName, string comments)
        {
            JobInstance job = CreateJobInstance(null, GetJobDefinitionName(), queueName, comments);

            job.Parameters[Constants.JobParameterExport].Value = parameters;

            return job;
        }

        private string GetJobDefinitionName()
        {
            return EntityFactory.CombineName(EntityType.JobDefinition, Registry.ContextManager.Configuration.FederationName, typeof(ExportTablesJob).Name);
        }

        public ExportTablesJobSettings GetJobDefinitionSettings()
        {
            var ef = new EntityFactory(Context);
            var jd = ef.LoadEntity<JobDefinition>(GetJobDefinitionName());
            return new ExportTablesJobSettings(jd.Settings);
        }
    }
}
