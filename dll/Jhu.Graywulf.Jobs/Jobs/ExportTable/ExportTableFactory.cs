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
    public class ExportTableFactory : JobFactoryBase
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

        public ExportTableFactory()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public ExportTableFactory(Context context)
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

        public JobInstance ScheduleAsJob(TableOrView source, string path, FileFormatDescription format, string queueName, string comments)
        {
            var job = GetInitializedJobInstance(queueName, comments);

            var et = new ExportTable();

            et.Source = source;

            // TODO: change this to support different compression formats
            // tar.gz won't work with streaming data, so use zip instead!
            path = Path.Combine(path, String.Format("{0}_{1}_{2}{3}.gz", Context.UserName, job.JobID, source.ObjectName, format.DefaultExtension));

            var destination = FileFormatFactory.CreateFile(format);
            destination.Uri = new Uri(String.Format("file:///{0}", path));
            destination.FileMode = DataFileMode.Write;
            // TODO: test this and delete if works destination.Compression = DataFileCompression.GZip;

            // special initialization in case of a text file
            if (destination is TextDataFileBase)
            {
                var tf = (TextDataFileBase)destination;
                tf.Encoding = Encoding.ASCII;
                tf.Culture = System.Globalization.CultureInfo.InvariantCulture;
                tf.GenerateIdentityColumn = false;
                tf.ColumnNamesInFirstLine = true;
            }

            et.Destination = destination;

            job.Parameters["Parameters"].SetValue(et);

            return job;
        }

        private JobInstance GetInitializedJobInstance(string queueName, string comments)
        {
            JobInstance job = CreateJobInstance(
                String.Format("{0}.{1}", Federation.AppSettings.FederationName, typeof(ExportTableJob).Name),
                queueName,
                comments);

            job.Name = String.Format("{0}_{1}", Context.UserName, job.JobID);
            job.Comments = comments;

            return job;
        }
    }
}
