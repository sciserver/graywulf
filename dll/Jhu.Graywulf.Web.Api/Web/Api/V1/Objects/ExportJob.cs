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

        private string[] tables;
        private FileFormat fileFormat;
        private Uri uri;
        private Credentials credentials;

        #endregion
        #region Properties

        [DataMember(Name = "tables")]
        [Description("An array of fully qualified names of tables to be exported.")]
        public string[] Tables
        {
            get { return tables; }
            set { tables = value; }
        }

        [DataMember(Name = "fileFormat", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Format of the files. Overrides format infered from extension.")]
        public FileFormat FileFormat
        {
            get { return fileFormat; }
            set { fileFormat = value; }
        }

        [DataMember(Name = "uri")]
        [Description("URI of the target file.")]
        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        [DataMember(Name = "credentials", EmitDefaultValue=false)]
        [Description("Credentials to access the target URI.")]
        [DefaultValue(null)]
        public Credentials Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Used for displaying the list of tables on the web UI.
        /// </remarks>
        [IgnoreDataMember]
        public string TableList
        {
            get
            {
                if (tables != null)
                {
                    string res = "";
                    for (int i = 0; i < tables.Length; i++)
                    {
                        if (i > 0)
                        {
                            res += ", ";
                        }

                        res += tables[i];
                    }

                    return res;
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        #endregion
        #region Constructors and initializers

        public ExportJob()
        {
            InitializeMembers();
        }

        public static ExportJob FromJobInstance(JobInstance jobInstance)
        {
            var job = new ExportJob();
            job.LoadFromRegistryObject(jobInstance);

            return job;
        }

        private void InitializeMembers()
        {
            base.Type = JobType.Export;

            this.tables = null;
            this.fileFormat = null;
            this.uri = null;
            this.credentials = null;
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

                // TODO:
                this.tables = new string[] { "xxx" };
                this.fileFormat = new FileFormat()
                {
                    MimeType = xr.GetAttribute("/ExportTablesParameters/Destinations/DataFileBase", "z:Type")
                };
                this.uri = new Uri(xr.GetXmlInnerText("ExportTablesParameters/Uri"));

                // TODO:
                // jobDescription.SchemaName = GetXmlInnerText(xml, "ExportTables/Sources/TableOrView/SchemaName");
                // jobDescription.ObjectName = GetXmlInnerText(xml, "ExportTables/Sources/TableOrView/ObjectName");
                // jobDescription.Path = GetXmlInnerText(xml, "ExportTables/Destinations/DataFileBase/Uri");
            }
        }

        public ExportTablesParameters CreateParameters(FederationContext context)
        {
            SourceTableQuery[] sources = null;
            IO.Credentials credentials = null;

            // Verify file format
            if (FileFormat == null || String.IsNullOrWhiteSpace(FileFormat.MimeType))
            {
                throw new ArgumentException("File format must be specified for export"); // TODO ***
            }

            // Verify source list
            if (tables == null || tables.Length == 0)
            {
                throw new ArgumentException("At least one table must be specified"); // TODO ***
            }

            sources = new SourceTableQuery[tables.Length];
            for (int i = 0; i < tables.Length; i++)
            {
                TableOrView table;
                if (!Util.SqlParser.TryParseTableName(context, tables[i], out table))
                {
                    throw new ArgumentException("Invalid table name");    // TODO ***
                }
                sources[i] = SourceTableQuery.Create(table);
            }

            if (Credentials != null)
            {
                credentials = Credentials.GetCredentials(context);
            }

            var ff = ExportTablesJobFactory.Create(context.Federation);
            return ff.CreateParameters(context.Federation, uri, credentials, sources, FileFormat.MimeType);
        }

        public override void Schedule(FederationContext context)
        {
            var p = CreateParameters(context);

            var ef = ExportTablesJobFactory.Create(context.Federation);
            var job = ef.ScheduleAsJob(p, GetQueueName(context), Comments);

            job.Save();

            LoadFromRegistryObject(job);
        }
    }
}
