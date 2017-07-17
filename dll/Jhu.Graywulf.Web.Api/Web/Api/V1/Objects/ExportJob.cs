using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ComponentModel;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Jobs.ExportTables;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Web.UI;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a table export job.")]
    public class ExportJob : Job
    {
        #region Private member variables

        private Uri uri;
        private Credentials credentials;
        private FileFormat fileFormat;
        private SourceTable source;

        #endregion
        #region Properties

        [DataMember(Name = "uri")]
        [Description("URI of the target file.")]
        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        [DataMember(Name = "credentials", EmitDefaultValue = false)]
        [Description("Credentials to access the target URI.")]
        [DefaultValue(null)]
        public Credentials Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }

        [DataMember(Name = "fileFormat", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Format of the files. Overrides format infered from extension.")]
        public FileFormat FileFormat
        {
            get { return fileFormat; }
            set { fileFormat = value; }
        }

        [DataMember(Name = "source", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Source table")]
        public SourceTable Source
        {
            get { return source; }
            set { source = value; }
        }

        #endregion
        #region Constructors and initializers

        public ExportJob()
        {
            InitializeMembers();
        }

        public static new ExportJob FromJobInstance(JobInstance jobInstance)
        {
            var job = new ExportJob();
            job.LoadFromRegistryObject(jobInstance);

            return job;
        }

        private void InitializeMembers()
        {
            base.Type = JobType.Export;

            this.uri = null;
            this.fileFormat = null;
            this.credentials = null;
            this.source = null;
        }

        #endregion

        protected override void LoadFromRegistryObject(JobInstance jobInstance)
        {
            base.LoadFromRegistryObject(jobInstance);

            // Because job parameter type might come from an unknown 
            // assembly, instead of deserializing, read xml directly here

            if (jobInstance.Parameters.ContainsKey(Jhu.Graywulf.Jobs.Constants.JobParameterExport))
            {
                var xml = new XmlDocument();
                xml.LoadXml(jobInstance.Parameters[Jhu.Graywulf.Jobs.Constants.JobParameterExport].XmlValue);

                var xr = new Util.XmlReader(xml);

                // URI
                // Try to take uri from the root (in case of archives) or from the first destination
                var uristring = xr.GetXmlInnerText("ExportTablesParameters/Uri");

                if (String.IsNullOrWhiteSpace(uristring))
                {
                    uristring = xr.GetXmlInnerText("ExportTablesParameters/Destinations/DataFileBase/Uri");
                }

                this.uri = new Uri(uristring, UriKind.RelativeOrAbsolute);

                // Source
                this.source = new SourceTable()
                {
                    Dataset = xr.GetXmlInnerText("ExportTablesParameters/Sources/SourceTableQuery/Dataset/Name"),
                    Table = Util.SqlParser.CombineTableName(
                        xr.GetXmlInnerText("ExportTablesParameters/Sources/SourceTableQuery/SchemaName"),
                        xr.GetXmlInnerText("ExportTablesParameters/Sources/SourceTableQuery/ObjectName"))
                };

                // Format
                this.fileFormat = GetFileFormat(JobInstance.RegistryContext, this.uri);
            }
        }

        public ExportTablesParameters CreateParameters(FederationContext context)
        {
            // Verify file format
            if (FileFormat == null || String.IsNullOrWhiteSpace(FileFormat.MimeType))
            {
                throw new ArgumentException("File format must be specified for export"); // TODO ***
            }

            // Source
            if (source == null)
            {
                throw new InvalidOperationException("Source must be specified"); // TODO ***
            }

            var sourcequery = source.GetSourceTableQuery(context);

            // Credentials
            IO.Credentials targetcredentials = null;

            if (Credentials != null)
            {
                targetcredentials = Credentials.GetCredentials(context);
            }

            var ff = ExportTablesJobFactory.Create(context.Federation);
            return ff.CreateParameters(context.Federation, uri, targetcredentials, sourcequery, FileFormat.MimeType);
        }

        protected internal override void Schedule(FederationContext context, string queueName)
        {
            var p = CreateParameters(context);

            var ef = ExportTablesJobFactory.Create(context.Federation);
            var job = ef.ScheduleAsJob(p, queueName, TimeSpan.Zero, Comments);

            job.Save();

            LoadFromRegistryObject(job);
        }
    }
}
