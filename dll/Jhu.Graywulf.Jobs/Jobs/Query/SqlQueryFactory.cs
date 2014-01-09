using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Activities;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    public class SqlQueryFactory : QueryFactory
    {
        /*
         * TODO: delete
         * public enum Settings
        {
            HotDatabaseVersionName,
            StatDatabaseVersionName,
            DefaultSchemaName,
            DefaultDatasetName,
            DefaultTableName,
            TemporarySchemaName,
            LongQueryTimeout,
        }*/

        public SqlQueryFactory()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public SqlQueryFactory(Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        protected override Type[] LoadQueryTypes()
        {
            return new Type[] { typeof(SqlQuery) };
        }

        protected override QueryBase CreateQueryBase(Node root)
        {
            QueryBase res;
            if (root is SelectStatement)
            {
                res = new SqlQuery(Context);
            }
            else
            {
                throw new NotImplementedException();
            }

            return res;
        }

        public override ParserLib.Parser CreateParser()
        {
            return new SqlParser.SqlParser();
        }

        public override SqlValidator CreateValidator()
        {
            return new SqlParser.SqlValidator();
        }

        public override SqlNameResolver CreateNameResolver()
        {
            return new SqlParser.SqlNameResolver();
        }

        protected override void GetInitializedQuery_Graywulf(QueryBase query, string queryString, string outputTable)
        {
            var federationname = Federation.AppSettings.FederationName;

            var ef = new EntityFactory(Context);

            var federation = ef.LoadEntity<Federation>(federationname);

            var jd = ef.LoadEntity<JobDefinition>(federationname, typeof(SqlQueryJob).Name);

            var user = new User(Context);
            user.Guid = Context.UserGuid;
            user.Load();

            query.ExecutionMode = ExecutionMode.Graywulf;
            query.FederationReference.Name = federationname;
            query.QueryString = queryString;

            query.SourceDatabaseVersionName = jd.Settings.HotDatabaseVersionName;
            query.StatDatabaseVersionName = jd.Settings.StatDatabaseVersionName;
            
            query.QueryTimeout = jd.Settings.QueryTimeout;

            // Add MyDB as custom source
            var mydbds = new GraywulfDataset();
            mydbds.Name = jd.Settings.DefaultDatasetName;
            mydbds.DefaultSchemaName = jd.Settings.DefaultSchemaName;
            mydbds.DatabaseInstance.Value = user.GetUserDatabaseInstance(federation.MyDBDatabaseVersion);
            mydbds.CacheSchemaConnectionString();
            mydbds.IsMutable = true;
            query.CustomDatasets.Add(mydbds);

            query.DefaultDataset = mydbds;

            // Set up MYDB for destination
            // ****** TODO add output table name to settings */
            query.Destination = new DestinationTable(
                mydbds,
                mydbds.DatabaseName,
                jd.Settings.DefaultSchemaName,
                String.IsNullOrWhiteSpace(outputTable) ? "outputtable" : outputTable,
                TableInitializationOptions.Drop | TableInitializationOptions.Create);

            // Set up temporary database
            var tempds = new GraywulfDataset();
            tempds.Name = Registry.Constants.TempDbName;
            tempds.IsOnLinkedServer = false;
            tempds.DatabaseVersion.Value = federation.TempDatabaseVersion;
            query.TemporaryDataset = tempds;

            // Set up code database
            var codeds = new GraywulfDataset();
            codeds.Name = Registry.Constants.CodeDbName;
            codeds.IsOnLinkedServer = false;
            codeds.DatabaseVersion.Value = federation.CodeDatabaseVersion;
            query.CodeDataset = codeds;
        }

        protected override void GetInitializedQuery_SingleServer(QueryBase query, string queryString, string outputTable, SqlServerDataset mydbds, SqlServerDataset tempds, SqlServerDataset codeds)
        {
            query.ExecutionMode = ExecutionMode.SingleServer;
            query.QueryString = queryString;

            query.QueryTimeout = 7200;

            if (mydbds != null)
            {
                query.DefaultDataset = mydbds;

                // Add MyDB as custom source
                query.CustomDatasets.Add(mydbds);

                // Set up MYDB for destination
                query.Destination = new DestinationTable(
                    mydbds,
                    mydbds.DatabaseName,
                    mydbds.DefaultSchemaName,
                    "",  // *** TODO ?
                    TableInitializationOptions.Drop | TableInitializationOptions.Create);
            }

            // Set up temporary and code database
            query.TemporaryDataset = tempds;
            query.CodeDataset = codeds;
        }

        public override JobInstance ScheduleAsJob(QueryBase query, string queueName, string comments)
        {
            var job = CreateJobInstance(
                String.Format("{0}.{1}", Federation.AppSettings.FederationName, typeof(SqlQueryJob).Name),
                queueName,
                comments);

            job.Parameters["Query"].SetValue(query);

            return job;
        }

        public virtual Activity GetAsWorkflow(QueryBase query)
        {
            return new SqlQueryJob();
        }

        public virtual Dictionary<string, object> GetWorkflowParameters(QueryBase query)
        {
            return new Dictionary<string, object>()
            {
                { "Query", query },
                { "UserGuid", Guid.Empty },
                { "JobGuid", Guid.Empty },
            };
        }
    }
}
