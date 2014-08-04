using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ServiceModel;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Jobs.ExportTables;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    public class ExportJob : Job
    {
        #region Private member variables

        private string[] tables;
        private string mimeType;
        private Uri uri;

        #endregion
        #region Properties

        [DataMember(Name = "tables")]
        public string[] Tables
        {
            get { return tables; }
            set { tables = value; }
        }

        [DataMember(Name = "mimeType")]
        public string ContentType
        {
            get { return mimeType; }
            set { mimeType = value; }
        }

        [DataMember(Name = "uri")]
        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Used to display list of tables on the web UI.
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

        public ExportJob(JobInstance jobInstance)
            : base(jobInstance)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            base.Type = JobType.Export;

            this.tables = null;
            this.mimeType = null;
            this.uri = null;
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
            var ef = ExportTablesFactory.Create(context.Federation);

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

        private string GetTableName(TableOrView table)
        {
            return table.ObjectName;
        }

        public override void Schedule(FederationContext context)
        {
            var p = CreateParameters(context);

            var ef = ExportTablesFactory.Create(context.Federation);
            var job = ef.ScheduleAsJob(p, GetQueueName(context), Comments);

            job.Save();

            LoadFromRegistryObject(job);
        }
    }
}
