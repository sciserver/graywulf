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

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    public class SqlQueryFactory : QueryFactory
    {
        public enum Settings
        {
            HotDatabaseVersionName,
            StatDatabaseVersionName,
            DefaultSchemaName,
            DefaultDatasetName,
            DefaultTableName,
            TemporarySchemaName,
            LongQueryTimeout,
        }

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

            // Load settings
            // **** TODO: this always takes settings from SqlQueryJob!!
            var settings = Jhu.Graywulf.Registry.Util.LoadSettings<Settings>(jd.Settings);

            query.ExecutionMode = ExecutionMode.Graywulf;
            query.FederationReference.Name = federationname;
            query.QueryString = queryString;

            query.SourceDatabaseVersionName = settings[Settings.HotDatabaseVersionName];
            query.StatDatabaseVersionName = settings[Settings.StatDatabaseVersionName];
            
            query.QueryTimeout = int.Parse(settings[Settings.LongQueryTimeout]);

            // Add MyDB as custom source
            var mydbds = new GraywulfDataset();
            mydbds.Name = settings[Settings.DefaultDatasetName];
            mydbds.DefaultSchemaName = settings[Settings.DefaultSchemaName];
            mydbds.DatabaseInstance.Value = user.GetUserDatabaseInstance(federation.MyDBDatabaseVersion);
            mydbds.CacheSchemaConnectionString();
            query.CustomDatasets.Add(mydbds);

            query.DefaultDataset = mydbds;

            // Set up MYDB for destination
            // ****** TODO add output table name to settings */
            query.Destination.Table = new Table()
            {
                Dataset = mydbds,
                SchemaName = settings[Settings.DefaultSchemaName],
                TableName = String.IsNullOrWhiteSpace(outputTable) ? "outputtable" : outputTable
            };
            query.Destination.Operation = DestinationTableOperation.Drop | DestinationTableOperation.Create;

            // Set up temporary database
            var tempds = new GraywulfDataset();
            tempds.IsOnLinkedServer = false;
            tempds.DatabaseVersion.Value = federation.TempDatabaseVersion;
            query.TemporaryDataset = tempds;
            query.TemporaryDataset.DefaultSchemaName = settings[Settings.TemporarySchemaName];

            // Set up code database
            var codeds = new GraywulfDataset();
            codeds.Name = "Code";   //  *** TODO
            codeds.IsOnLinkedServer = false;
            codeds.DatabaseVersion.Value = federation.CodeDatabaseVersion;
            query.CodeDataset = codeds;
            query.CodeDataset.DefaultSchemaName = "dbo";    // *** TODO
        }

        protected override void GetInitializedQuery_SingleServer(QueryBase query, string queryString, string outputTable, SqlServerDataset mydbds, SqlServerDataset tempds)
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
                query.Destination.Table = new Table();
                query.Destination.Table.Dataset = mydbds;
                query.Destination.Table.DatabaseName = mydbds.DatabaseName;
                query.Destination.Table.SchemaName = mydbds.DefaultSchemaName;
                query.Destination.Operation = DestinationTableOperation.Drop | DestinationTableOperation.Create;
            }

            // Set up temporary database
            query.TemporaryDataset = tempds;
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
