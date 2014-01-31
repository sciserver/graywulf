using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Jobs.ExportTables;

namespace Jhu.Graywulf.Web.UI.Api
{
    public class ExportJob : Job
    {
        private string[] tables;
        private string format;
        private string uri;

        public override JobType Type
        {
            get { return JobType.Export; }
            set {  }
        }

        public string[] Tables
        {
            get { return tables; }
            set { tables = value; }
        }

        public string Format
        {
            get { return format; }
            set { format = value; }
        }

        public string Uri
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

        public ExportJob()
        {
            InitializeMembers();
        }

        public ExportJob(JobInstance jobInstance)
            :base(jobInstance)
        {
            InitializeMembers();

            CopyFromJobInstance(jobInstance);
        }

        private void InitializeMembers()
        {
            this.tables = null;
            this.format = null;
            this.uri = null;
        }

        private void CopyFromJobInstance(JobInstance jobInstance)
        {
            // Because job parameter type might come from an unknown 
            // assembly, instead of deserializing, read xml directly here

            if (jobInstance.Parameters.ContainsKey(Jhu.Graywulf.Jobs.Constants.JobParameterExport))
            {
                var xml = new XmlDocument();
                xml.LoadXml(jobInstance.Parameters[Jhu.Graywulf.Jobs.Constants.JobParameterExport].XmlValue);

                // TODO:
                // jobDescription.SchemaName = GetXmlInnerText(xml, "ExportTables/Sources/TableOrView/SchemaName");
                // jobDescription.ObjectName = GetXmlInnerText(xml, "ExportTables/Sources/TableOrView/ObjectName");
                // jobDescription.Path = GetXmlInnerText(xml, "ExportTables/Destinations/DataFileBase/Uri");
            }
        }

        public ExportTablesParameters CreateParameters(FederationContext context)
        {
            var ep = new ExportTablesParameters()
            {
                StreamFactoryType = context.Federation.StreamFactory,
                FileFormatFactoryType = context.Federation.FileFormatFactory,
                Uri = new Uri(uri, UriKind.Absolute),
                Archival = IO.DataFileArchival.Zip,
            };

            // Add tables and destination files
            var ts = new TableOrView[tables.Length];
            var ds = new DataFileBase[tables.Length];
            for (int i = 0; i < tables.Length; i++)
            {
                string schemaName, tableName;
                var parts = tables[i].Split('.');
                
                if (parts.Length == 1)
                {
                    schemaName = context.MyDBDataset.DefaultSchemaName;
                    tableName = parts[0];
                }
                else if (parts.Length == 2)
                {
                    schemaName = parts[0];
                    tableName = parts[1];
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Invalid table name: '{0}'", tables[i])); // TODO
                }

                ts[i] = context.MyDBDataset.Tables[context.MyDBDataset.DatabaseName, schemaName, tableName];
                ds[i] = context.FileFormatFactory.CreateFile(GetTableName(ts[i]) + format);
            }

            ep.Sources = ts;
            ep.Destinations = ds;

            return ep;
        }

        private string GetTableName(TableOrView table)
        {
            return table.ObjectName;
        }

        public override JobInstance Schedule(FederationContext context)
        {
            var p = CreateParameters(context);

            var ef = ExportTablesFactory.Create(context.Federation);
            var job = ef.ScheduleAsJob(p, GetQueueName(context), Comments);

            job.Save();

            return job;
        }
    }
}
