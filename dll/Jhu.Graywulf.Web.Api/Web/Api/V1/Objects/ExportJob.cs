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

                // Table
                // Take exported table name from the first srouce object
                var datasetname = xr.GetXmlInnerText("ExportTablesParameters/Sources/SourceTableQuery/Dataset/Name");
                var schemaname = xr.GetXmlInnerText("ExportTablesParameters/Sources/SourceTableQuery/SourceSchemaName");
                var tablename = xr.GetXmlInnerText("ExportTablesParameters/Sources/SourceTableQuery/SourceObjectName");

                this.source = new SourceTable()
                {
                    Dataset = datasetname,
                    Table = schemaname +
                        (!String.IsNullOrWhiteSpace(schemaname) ? "." : "") +
                        tablename
                };
                
                // Format
                var ff = FileFormatFactory.Create(jobInstance.Context.Federation.FileFormatFactory);
                string filename, extension;
                DataFileCompression compression;
                DataFileBase file;
                ff.GetFileExtensions(this.uri, out filename, out extension, out compression);

                if (ff.TryCreateFileFromExtension(extension, out file))
                {
                    this.fileFormat = new V1.FileFormat()
                    {
                        MimeType = file.Description.MimeType
                    };
                }
            }
        }

        public ExportTablesParameters CreateParameters(FederationContext context)
        {
            IO.Credentials credentials = null;

            // Verify file format
            if (FileFormat == null || String.IsNullOrWhiteSpace(FileFormat.MimeType))
            {
                throw new ArgumentException("File format must be specified for export"); // TODO ***
            }

            if (source == null || 
                String.IsNullOrWhiteSpace(source.Table) ||
                String.IsNullOrWhiteSpace(source.Dataset))
            {
                throw new ArgumentException("Source must be specified"); // TODO ***
            }
            
            // Parse table name and create source object
            TableOrView tab;
            if (!Util.SqlParser.TryParseTableName(context, source.Table, out tab))
            {
                throw new ArgumentException("Invalid table name");    // TODO ***
            }

            var dataset = context.SchemaManager.Datasets[source.Dataset];

            // Make sure dataset is a user dataset, do not allow export from big catalogs
            if (!dataset.IsMutable)
            {
                throw new ArgumentException("Cannot export data from the specified dataset.");  // TODO ***
            }

            tab.Dataset = dataset;

            var sourcequery = SourceTableQuery.Create(tab);

            if (Credentials != null)
            {
                credentials = Credentials.GetCredentials(context);
            }

            var ff = ExportTablesJobFactory.Create(context.Federation);
            return ff.CreateParameters(context.Federation, uri, credentials, sourcequery, FileFormat.MimeType);
        }

        public override void Schedule(FederationContext context)
        {
            var p = CreateParameters(context);

            var ef = ExportTablesJobFactory.Create(context.Federation);
            var job = ef.ScheduleAsJob(p, GetQueueName(context), TimeSpan.Zero, Comments);

            job.Save();

            LoadFromRegistryObject(job);
        }
    }
}
