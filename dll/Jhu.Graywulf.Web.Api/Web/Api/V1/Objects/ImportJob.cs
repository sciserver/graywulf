using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.Schema;
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
        private DestinationTable destination;
        private ImportOptions options;

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

        [DataMember(Name = "destination", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Destination table")]
        public DestinationTable Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        [DataMember(Name = "options", EmitDefaultValue = false)]
        [Description("Optional settings of the table import.")]
        [DefaultValue(null)]
        public ImportOptions Options
        {
            get { return options; }
            set { options = value; }
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
            this.destination = null;
            this.options = null;
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

                // URI
                // Try to take uri from the root (in case of archives) or from the first source
                var uristring = xr.GetXmlInnerText("ImportTablesParameters/Uri");
                if (String.IsNullOrWhiteSpace(uristring))
                {
                    uristring = xr.GetXmlInnerText("ImportTablesParameters/Sources/DataFileBase/Uri");
                }

                this.uri = new Uri(uristring, UriKind.RelativeOrAbsolute);

                // Destination
                this.destination = new DestinationTable()
                {
                    Dataset = xr.GetXmlInnerText("ImportTablesParameters/Destinations/DestinationTable/Dataset/Name"),
                    Table = Util.SqlParser.CombineTableName(
                        xr.GetXmlInnerText("ImportTablesParameters/Destinations/DestinationTable/SchemaName"),
                        xr.GetXmlInnerText("ImportTablesParameters/Destinations/DestinationTable/TableNamePattern"))
                };

                // Format
                this.fileFormat = GetFileFormat(JobInstance.RegistryContext, this.uri);

                // Options
                bool gid;
                xr.TryGetXmlBoolean("ImportTablesParameters/Options/GenerateIdentityColumn", out gid);
                this.options = new ImportOptions()
                {
                    GenerateIdentityColumn = gid
                };
            }
        }
        
        public ImportTablesParameters CreateParameters(FederationContext context)
        {
            DataFileBase source = null;
            IO.Credentials credentials = null;
            IO.Tasks.ImportTableOptions options = null;

            // Source
            if (FileFormat != null)
            {
                source = FileFormat.GetDataFile(context, uri);
            }

            // Destination
            if (destination == null)
            {
                throw new InvalidOperationException("Destination must be specified"); // TODO ***
            }

            var destinationtable = destination.GetDestinationTable(context);

            // Credentials
            if (Credentials != null)
            {
                credentials = Credentials.GetCredentials(context);
            }

            if (Options != null)
            {
                options = Options.GetOptions();
            }

            var ff = ImportTablesJobFactory.Create(context.Federation);
            var par = ff.CreateParameters(context.Federation, uri, credentials, source, destinationtable, options);

            return par;
        }

        protected internal override void OnSchedule(FederationContext context, string queueName)
        {
            var p = CreateParameters(context);

            var ff = ImportTablesJobFactory.Create(context.Federation);
            var job = ff.ScheduleAsJob(p, queueName, TimeSpan.Zero, Comments);

            job.Save();

            LoadFromRegistryObject(job);
        }
    }
}
