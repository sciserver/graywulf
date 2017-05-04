using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.ServiceModel;
using System.ComponentModel;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a query job.")]
    public class QueryJob : Job
    {
        #region Private member variables

        private string query;
        private string output;

        #endregion
        #region Properties

        [DataMember(Name = "query")]
        [Description("Query text in SQL.")]
        public string Query
        {
            get { return query; }
            set { query = value; }
        }

        [DataMember(Name = "output", EmitDefaultValue=false)]
        [DefaultValue(null)]
        [Description("Output table")]
        public string Output
        {
            get { return output; }
            set { output = value; }
        }

        #endregion
        #region Constructors and initializers

        public QueryJob()
        {
            InitializeMembers();
        }

        public QueryJob(string query, JobQueue queue)
            : this()
        {
            InitializeMembers();

            this.query = query;
            this.Queue = queue;
        }

        public static new QueryJob FromJobInstance(JobInstance jobInstance)
        {
            var job = new QueryJob();
            job.LoadFromRegistryObject(jobInstance);

            return job;
        }

        private void InitializeMembers()
        {
            base.Type = JobType.Query;

            this.query = null;
        }

        #endregion

        protected override void LoadFromRegistryObject(JobInstance jobInstance)
        {
            base.LoadFromRegistryObject(jobInstance);

            // Because job parameter type might come from an unknown 
            // assembly, instead of deserializing, read xml directly here

            if (jobInstance.Parameters.ContainsKey(Jhu.Graywulf.Jobs.Constants.JobParameterQuery))
            {
                var xml = new XmlDocument();
                xml.LoadXml(jobInstance.Parameters[Jhu.Graywulf.Jobs.Constants.JobParameterQuery].XmlValue);

                var xr = new Util.XmlReader(xml);

                this.query = xr.GetXmlInnerText("Query/QueryString");

                var datasetName = xr.GetXmlInnerText("Query/Output/Dataset/Name");
                var schemaName = xr.GetXmlInnerText("Query/Output/SchemaName");
                var tableName = xr.GetXmlInnerText("Query/Output/ObjectName");

                string name = String.Empty;

                if (datasetName != null)
                {
                    name += datasetName + ":";
                }

                if (schemaName != null)
                {
                    name += schemaName + ".";
                }

                if (tableName != null)
                {
                    name += tableName;
                }

                if (name.Length > 0)
                {
                    this.output = name;
                }
                else
                {
                    this.output = null;
                }
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
        public SqlQuery CreateQuery(FederationContext context)
        {
            // TODO: pass default target dataset and table name

            var qf = QueryFactory.Create(context.Federation);
            var q = qf.CreateQuery(query);

            var udf = UserDatabaseFactory.Create(context);
            var udbs = udf.GetUserDatabases(context.RegistryUser);
            var usis = udf.GetUserDatabaseServerInstances(context.RegistryUser);

            foreach (var key in udbs.Keys)
            {
                qf.AppendUserDatabase(q, udbs[key], usis[key]);
            }

            // TODO: Target table settings will need to be modified
            // once multi-select queries are implemented
            q.BatchName = null;
            q.QueryName = this.Name;

            // Set up default destination which might be overriden
            // by an INTO clause
            var userdb = (SqlServerDataset)context.SchemaManager.Datasets[Registry.Constants.UserDbName];

            q.Destination = new IO.Tasks.DestinationTable()
            {
                Dataset = userdb,
                DatabaseName = userdb.DatabaseName,
                SchemaName = userdb.DefaultSchemaName
            };

            switch (Queue)
            {
                default:
                case JobQueue.Quick:
                    q.Destination.Options = TableInitializationOptions.Drop | TableInitializationOptions.Create;
                    q.Destination.TableNamePattern = Jhu.Graywulf.Jobs.Constants.DefaultQuickResultsTableNamePattern;
                    break;
                case JobQueue.Long:
                    q.Destination.Options = TableInitializationOptions.Create | TableInitializationOptions.GenerateUniqueName;
                    q.Destination.TableNamePattern = Jhu.Graywulf.Jobs.Constants.DefaultLongResultsTableNamePattern;
                    break;
            }

            return q;
        }

        /// <summary>
        /// Schedules a new query job based on the job settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override void Schedule(FederationContext context)
        {
            // Create the query object from the query test
            var query = CreateQuery(context);

            // Run syntax and other sanity tests
            query.Verify();

            // Schedule as a job
            var qf = QueryFactory.Create(context.Federation);

            this.JobInstance = qf.ScheduleAsJob(this.Name, query, GetQueueName(context), TimeSpan.Zero, Comments);
            this.JobInstance.Save();

            // Save dependencies
            SaveDependencies();

            // Reload
            LoadFromRegistryObject(this.JobInstance);
        }
    }
}
