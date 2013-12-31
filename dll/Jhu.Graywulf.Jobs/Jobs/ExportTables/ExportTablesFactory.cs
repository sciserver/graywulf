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

namespace Jhu.Graywulf.Jobs.ExportTable
{
    [Serializable]
    public class ExportTablesFactory : JobFactoryBase
    {
        [NonSerialized]
        protected static object syncRoot = new object();

        [NonSerialized]
        private static Type[] fileFormats = null;

        public Type[] FileFormats
        {
            get
            {
                lock (syncRoot)
                {
                    if (fileFormats == null)
                    {
                        fileFormats = LoadFileFormats();
                    }
                }

                return fileFormats;
            }
        }

        public ExportTablesFactory()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public ExportTablesFactory(Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        protected virtual Type[] LoadFileFormats()
        {
            // TODO: write this one to use config
            return new Type[] { typeof(Jhu.Graywulf.Format.DelimitedTextDataFile) };
        }

        public JobInstance ScheduleAsJob(TableOrView[] sources, string path, FileFormatDescription format, string queueName, string comments)
        {
            var job = GetInitializedJobInstance(queueName, comments);

            path = Path.Combine(path, String.Format("{0}_{1}.{2}", Context.UserName, job.JobID, Jhu.Graywulf.IO.Constants.FileExtensionZip));

            var destinations = new DataFileBase[sources.Length];
            for (int i = 0; i < sources.Length; i++)
            {
                var ff = FileFormatFactory.Create();

                var dest = ff.CreateFile(format);
                dest.Uri = Util.UriConverter.FromFilePath(String.Format("{0}{1}", sources[i].ObjectName, format.DefaultExtension));

                // special initialization in case of a text file
                // TODO: move this somewhere else, maybe web?
                if (dest is TextDataFileBase)
                {
                    var tf = (TextDataFileBase)dest;
                    tf.Encoding = Encoding.ASCII;
                    tf.Culture = System.Globalization.CultureInfo.InvariantCulture;
                    tf.GenerateIdentityColumn = false;
                    tf.ColumnNamesInFirstLine = true;
                }

                destinations[i] = dest;
            }

            var et = new ExportTables()
            {
                Sources = sources,
                Destinations = destinations,
                Archival = DataFileArchival.Zip,
                Uri = Util.UriConverter.FromFilePath(path),
            };

            job.Parameters["Parameters"].SetValue(et);

            return job;
        }

        private JobInstance GetInitializedJobInstance(string queueName, string comments)
        {
            JobInstance job = CreateJobInstance(
                String.Format("{0}.{1}", Federation.AppSettings.FederationName, typeof(ExportTablesJob).Name),
                queueName,
                comments);

            job.Name = String.Format("{0}_{1}", Context.UserName, job.JobID);
            job.Comments = comments;

            return job;
        }
    }
}
