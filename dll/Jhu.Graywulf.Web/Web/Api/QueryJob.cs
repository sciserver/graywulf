using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.ServiceModel;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.Api
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

        public QueryJob(string query, JobQueue queue)
            : this()
        {
            this.query = query;
            this.Queue = queue;
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

        /// <summary>
        /// Creates a new QueryBase object based on the job settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public QueryBase CreateQuery(FederationContext context)
        {
            var qf = QueryFactory.Create(context.Federation);
            var q = qf.CreateQuery(query, ExecutionMode.Graywulf);

            switch (Queue)
            {
                case JobQueue.Quick:
                    q.Destination = new IO.Tasks.DestinationTable(
                        context.MyDBDataset,
                        context.MyDBDataset.DatabaseName,
                        context.MyDBDataset.DefaultSchemaName,
                        "quickResults",
                        TableInitializationOptions.Drop | TableInitializationOptions.Create);
                    break;
                case JobQueue.Long:
                    break;
                default:
                    break;
            }

            return q;
        }

        /// <summary>
        /// Schedules a new query job based on the job settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public JobInstance Schedule(FederationContext context)
        {
            var q = CreateQuery(context);
            q.Verify();

            

            var qf = QueryFactory.Create(context.Federation);
            var job = qf.ScheduleAsJob(q, GetQueueName(context), Comments);

            job.Save();

            return job;
        }
    }
}
