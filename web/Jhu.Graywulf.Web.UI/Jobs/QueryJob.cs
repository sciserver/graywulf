using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.ServiceModel;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.UI.Jobs
{
    [DataContract]
    public class QueryJob : Job
    {
        private string query;

        public override JobType Type
        {
            get { return JobType.Query; }
            set { }
        }

        public string Query
        {
            get { return query; }
            set { query = value; }
        }

        public QueryJob()
        {
            InitializeMembers();
        }

        public QueryJob(JobInstance jobInstance)
            : base(jobInstance)
        {
            InitializeMembers();

            CopyFromJobInstance(jobInstance);
        }

        private void InitializeMembers()
        {
            this.query = null;
        }

        private void CopyFromJobInstance(JobInstance jobInstance)
        {
            // Because job parameter type might come from an unknown 
            // assembly, instead of deserializing, read xml directly here

            if (jobInstance.Parameters.ContainsKey(Jhu.Graywulf.Jobs.Constants.JobParameterQuery))
            {
                var xml = new XmlDocument();
                xml.LoadXml(jobInstance.Parameters[Jhu.Graywulf.Jobs.Constants.JobParameterQuery].XmlValue);

                this.query = GetXmlInnerText(xml, "Query/QueryString");
                //jobDescription.SchemaName = GetXmlInnerText(xml, "Query/Destination/SchemaName");
                //jobDescription.ObjectName = GetXmlInnerText(xml, "Query/Destination/TableName");
            }
            else
            {
                // TODO
                // This is probably a wrong job in the database
            }
        }
    }
}
