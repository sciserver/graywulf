using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using System.ServiceModel;
using System.ComponentModel;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.Jobs.Query;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a query job.")]
    public class QueryJob : Job
    {
        #region Private member variables

        private string query;
        private string[] output;

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
        [Description("Output tables")]
        public string[] Output
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

        public QueryJob(string query)
            : this()
        {
            InitializeMembers();

            this.query = query;
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
            this.output = null;
        }

        #endregion

        protected override void LoadFromRegistryObject(JobInstance jobInstance)
        {
            base.LoadFromRegistryObject(jobInstance);

            // Because job parameter type might come from an unknown 
            // assembly, instead of deserializing, read xml directly here

            if (jobInstance.Parameters.ContainsKey(Jhu.Graywulf.Registry.Constants.JobParameterQuery))
            {
                LoadFromRegistry_V1_3(jobInstance);
            }
            else if (jobInstance.Parameters.ContainsKey(Jhu.Graywulf.Registry.Constants.JobParameterParameters))
            {
                LoadFromRegistry_V1_4(jobInstance);
            }
            else
            {
                // TODO
                // This is probably a wrong job in the database
            }
        }

        private void LoadFromRegistry_V1_3(JobInstance jobInstance)
        {
            var xml = new XmlDocument();
            xml.LoadXml(jobInstance.Parameters[Jhu.Graywulf.Registry.Constants.JobParameterQuery].XmlValue);

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
                this.output = new string[] { name };
            }
            else
            {
                this.output = null;
            }
        }

        private void LoadFromRegistry_V1_4(JobInstance jobInstance)
        {
            var xml = new XmlDocument();
            xml.LoadXml(jobInstance.Parameters[Jhu.Graywulf.Registry.Constants.JobParameterParameters].XmlValue);

            var xr = new Util.XmlReader(xml);
            var outputs = new List<string>();
            

            foreach (var t in xr.EnumerateAsDictionary("SqlQueryParameters/OutputTables/Table"))
            {
                var table = new Sql.Schema.Table()
                {
                    Dataset = new SqlServerDataset()
                    {
                        Name = t.ContainsKey("DatasetName") ? t["DatasetName"] : null,

                    },
                    DatabaseName = t.ContainsKey("DatabaseName") ? t["DatabaseName"] : null,
                    SchemaName = t.ContainsKey("SchemaName") ? t["SchemaName"] : null,
                    ObjectName = t.ContainsKey("ObjectName") ? t["ObjectName"] : null,
                };
                
                outputs.Add(table.Dataset.GetObjectFullyResolvedName(table));
            }

            this.query = xr.GetXmlInnerText("SqlQueryParameters/QueryString");
            this.output = outputs.ToArray();
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
            
            q.Parameters.FederationName = context.Federation.GetFullyQualifiedName();
            q.Parameters.BatchName = null;
            q.Parameters.QueryName = this.Name;

            // Set up default destination which might be overriden
            // by an INTO clause
            var userdb = (SqlServerDataset)context.SchemaManager.Datasets[Registry.Constants.UserDbName];

            // Pass default target dataset and table name
            q.Parameters.DefaultOutputDataset = userdb;
            q.Parameters.DefaultSourceDataset = userdb;
            
            q.Parameters.Destination = new IO.Tasks.DestinationTable()
            {
                Dataset = userdb,
                DatabaseName = userdb.DatabaseName,
                SchemaName = userdb.DefaultSchemaName
            };

            // TODO: Target table settings will need to be modified
            // once multi-select queries are implemented

            switch (Queue)
            {
                default:
                case JobQueue.Quick:
                    q.Parameters.Destination.Options = TableInitializationOptions.Drop | TableInitializationOptions.Create;
                    q.Parameters.Destination.TableNamePattern = Jhu.Graywulf.Sql.Jobs.Query.Constants.DefaultQuickResultsTableNamePattern;
                    break;
                case JobQueue.Long:
                    q.Parameters.Destination.Options = TableInitializationOptions.Create | TableInitializationOptions.GenerateUniqueName;
                    q.Parameters.Destination.TableNamePattern = Jhu.Graywulf.Sql.Jobs.Query.Constants.DefaultLongResultsTableNamePattern;
                    break;
            }

            return q;
        }

        /// <summary>
        /// Schedules a new query job based on the job settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected internal override void OnSchedule(FederationContext context, string queueName)
        {
            // Create the query object from the query test
            var query = CreateQuery(context);

            // Run syntax and other sanity tests
            query.Verify();

            // Schedule as a job
            var qf = QueryFactory.Create(context.Federation);

            this.JobInstance = qf.ScheduleAsJob(this.Name, query, queueName, TimeSpan.Zero, Comments);
            this.JobInstance.Save();

            // Save dependencies
            SaveDependencies();

            // Reload
            LoadFromRegistryObject(this.JobInstance);
        }
    }
}
