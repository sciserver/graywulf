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

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a table export job.")]
    public class ExportJob : Job
    {
        #region Private member variables

        private string[] tables;
        private string mimeType;
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

        [DataMember(Name = "mimeType")]
        [Description("Mime type of the target data format.")]
        public string ContentType
        {
            get { return mimeType; }
            set { mimeType = value; }
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
            this.mimeType = null;
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

                this.tables = new string[] { "xxx" };
                this.mimeType = GetAttribute(xml, "/ExportTablesParameters/Destinations/DataFileBase", "z:Type");
                this.uri = new Uri(GetXmlInnerText(xml, "ExportTablesParameters/Uri"));

                // TODO:
                // jobDescription.SchemaName = GetXmlInnerText(xml, "ExportTables/Sources/TableOrView/SchemaName");
                // jobDescription.ObjectName = GetXmlInnerText(xml, "ExportTables/Sources/TableOrView/ObjectName");
                // jobDescription.Path = GetXmlInnerText(xml, "ExportTables/Destinations/DataFileBase/Uri");
            }
        }

        public ExportTablesParameters CreateParameters(FederationContext context)
        {
            var ef = ExportTablesJobFactory.Create(context.Federation);

            // Add tables and destination files
            var ts = new TableOrView[tables.Length];

            // Table names are specified as string, so we need to parse them
            var parser = new SqlParser.SqlParser();
            var nr = new SqlNameResolver()
            {
                SchemaManager = context.SchemaManager
            };

            for (int i = 0; i < tables.Length; i++)
            {
                var tn = (SqlParser.TableOrViewName)parser.Execute(new SqlParser.TableOrViewName(), tables[i]);
                var tr = tn.TableReference;
                tr.SubstituteDefaults(context.SchemaManager, context.MyDBDataset.Name);
                ts[i] = context.MyDBDataset.Tables[tr.DatabaseName, tr.SchemaName, tr.DatabaseObjectName];
            }

            return ef.CreateParameters(context.Federation, ts, uri, mimeType, GetQueueName(context), Comments);
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
