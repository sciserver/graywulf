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
    public class ExportTablesJobFactory : JobFactoryBase
    {
        #region Static members

        public static ExportTablesJobFactory Create(Federation federation)
        {
            return new ExportTablesJobFactory(federation.Context);
        }

        #endregion

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

        public ExportTablesParameters CreateParameters(Federation federation, TableOrView[] sources, Uri uri, string mimeType, string queueName, string comments)
        {
            var ff = FileFormatFactory.Create(federation.FileFormatFactory);

            // Create destination files
            // One file per source
            var destinations = new DataFileBase[sources.Length];
            for (int i = 0; i < sources.Length; i++)
            {
                // TODO: use file extensions instead of type?
                var destination = ff.CreateFileFromMimeType(mimeType);
                destination.Uri = Util.UriConverter.FromFilePath(sources[i].ObjectName + destination.Description.Extension);

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

            var ep = new ExportTablesParameters()
            {
                Sources = sources,
                Destinations = destinations,
                Archival = DataFileArchival.Zip,
                Uri = uri,
                FileFormatFactoryType = federation.FileFormatFactory,
                StreamFactoryType = federation.StreamFactory,
            };

            return ep;
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
