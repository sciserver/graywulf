using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Jobs.ImportTables;

namespace Jhu.Graywulf.Web.Api.V1
{
    [Description("Represents a data table import job.")]
    public class ImportJob : Job
    {
        #region Private member variables

        private Uri uri;
        private Credentials credentials;
        private FileFormat fileFormat;
        private string dataset;
        private string table;

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
        [Description("Credentials to access the source URI.")]
        [DefaultValue(null)]
        public Credentials Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }

        [DataMember(Name = "fileFormat", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Format of the file. Overrides format infered from extension.")]
        public FileFormat FileFormat
        {
            get { return fileFormat; }
            set { fileFormat = value; }
        }

        [DataMember(Name = "dataset")]
        [Description("Destination dataset.")]
        public string Dataset
        {
            get { return dataset; }
            set { dataset = value; }
        }

        [DataMember(Name = "table")]
        [Description("Destination table of the import.")]
        public string Table
        {
            get { return table; }
            set { table = value; }
        }

        #endregion
        #region Constructors and initializers

        public ImportJob()
        {
            InitializeMembers();
        }

        public static new ImportJob FromJobInstance(JobInstance jobInstance)
        {
            var job = new ImportJob();
            job.LoadFromRegistryObject(jobInstance);

            return job;
        }

        private void InitializeMembers()
        {
            base.Type = JobType.Import;

            this.uri = null;
            this.fileFormat = null;
            this.credentials = null;
            this.dataset = null;
            this.table = null;
        }

        #endregion

        protected override void LoadFromRegistryObject(JobInstance jobInstance)
        {
            base.LoadFromRegistryObject(jobInstance);

            if (jobInstance.Parameters.ContainsKey(Jhu.Graywulf.Jobs.Constants.JobParameterImport))
            {
                var xml = new XmlDocument();
                xml.LoadXml(jobInstance.Parameters[Jhu.Graywulf.Jobs.Constants.JobParameterImport].XmlValue);

                var xr = new Util.XmlReader(xml);

                // Try to take uri from the root (in case of archives) or from the first source
                var uristring = xr.GetXmlInnerText("ImportTablesParameters/Uri");
                if (String.IsNullOrWhiteSpace(uristring))
                {
                    uristring = xr.GetXmlInnerText("ImportTablesParameters/Sources/DataFileBase/Uri");
                }

                this.uri = new Uri(uristring, UriKind.RelativeOrAbsolute);

                // Take target table name from the first destination object
                var datasetname = xr.GetXmlInnerText("ImportTablesParameters/Destinations/DestinationTable/DatasetName");
                var schemaname = xr.GetXmlInnerText("ImportTablesParameters/Destinations/DestinationTable/SchemaName");
                var tablename = xr.GetXmlInnerText("ImportTablesParameters/Destinations/DestinationTable/TableNamePattern");

                this.dataset = datasetname;

                // TODO: use generic function
                this.table = schemaname +
                    (!String.IsNullOrWhiteSpace(schemaname) ? "." : "") +
                    tablename;

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

        public static DestinationTable GetDestinationTable(FederationContext context, string datasetName, string token)
        {
            string schemaName, tableName;

            if (Util.SqlParser.TryParseTableName(context, token, out schemaName, out tableName))
            {
                return GetDestinationTable(context, datasetName, schemaName, tableName);
            }
            else
            {
                return GetDestinationTable(context, datasetName, null, null);
            }
        }

        public static DestinationTable GetDestinationTable(FederationContext context, string datasetName, string schemaName, string tableName)
        {
            var userdb = (SqlServerDataset)context.SchemaManager.Datasets[datasetName];
            var destination = new DestinationTable(
                    userdb,
                    userdb.DatabaseName,
                    userdb.DefaultSchemaName,
                    IO.Constants.ResultsetNameToken,        // generate table names automatically
                    TableInitializationOptions.Create | TableInitializationOptions.GenerateUniqueName);

            if (!String.IsNullOrWhiteSpace(schemaName))
            {
                destination.SchemaName = schemaName;
            }

            if (!String.IsNullOrWhiteSpace(tableName))
            {
                destination.TableNamePattern = tableName;   // TODO: handle patterns?
            }

            return destination;
        }

        public DestinationTable GetDestinationTable(FederationContext context)
        {
            if (table != null)
            {
                string schemaName, tableName;
                if (Util.SqlParser.TryParseTableName(context, table, out schemaName, out tableName))
                {
                    return GetDestinationTable(context, dataset, schemaName, tableName);
                }
            }

            return GetDestinationTable(context, dataset, null, null);
        }



        public ImportTablesParameters CreateParameters(FederationContext context)
        {
            DataFileBase source = null;
            DestinationTable destination = null;
            IO.Credentials credentials = null;

            if (FileFormat != null)
            {
                source = FileFormat.GetDataFile(context, uri);
            }

            destination = GetDestinationTable(context);

            if (Credentials != null)
            {
                credentials = Credentials.GetCredentials(context);
            }


            var ff = ImportTablesJobFactory.Create(context.Federation);
            return ff.CreateParameters(context.Federation, uri, credentials, source, destination);
        }

        public override void Schedule(FederationContext context)
        {
            var p = CreateParameters(context);

            var ff = ImportTablesJobFactory.Create(context.Federation);
            var job = ff.ScheduleAsJob(p, GetQueueName(context), TimeSpan.Zero, Comments);

            job.Save();

            LoadFromRegistryObject(job);
        }
    }
}
