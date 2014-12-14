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
    /// <summary>
    /// Implements function to create a query job from a query string by
    /// configuring all necessary parameters.
    /// </summary>
    [Serializable]
    public class SqlQueryFactory : QueryFactory
    {
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

        /// <summary>
        /// Creates a query object based on the parsing tree root object.
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a parser object that produces a parsing tree this class can handle.
        /// </summary>
        /// <returns></returns>
        public override ParserLib.Parser CreateParser()
        {
            return new SqlParser.SqlParser();
        }

        /// <summary>
        /// Creates a validator object that can validate queries this class can handle.
        /// </summary>
        /// <returns></returns>
        public override SqlValidator CreateValidator()
        {
            return new SqlParser.SqlValidator();
        }

        /// <summary>
        /// Creates a name resolver object that can resolve identifiers in queirs
        /// this class can handle.
        /// </summary>
        /// <returns></returns>
        public override SqlNameResolver CreateNameResolver()
        {
            return new SqlParser.SqlNameResolver();
        }

        /// <summary>
        /// Initializes a query object for execution within the Graywulf framework.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="queryString"></param>
        /// <param name="outputTable"></param>
        /// <remarks>
        /// The initialized object will be the input parameter of a query job.
        /// </remarks>
        protected override void GetInitializedQuery_Graywulf(QueryBase query, string queryString, string outputTable)
        {
            var ef = new EntityFactory(Context);

            var federation = ef.LoadEntity<Federation>(Registry.AppSettings.FederationName);
            var jd = ef.LoadEntity<JobDefinition>(Registry.AppSettings.FederationName, typeof(SqlQueryJob).Name);

            var settings = new SqlQueryJobSettings(jd.Settings);

            var user = new User(Context);
            user.Guid = Context.UserGuid;
            user.Load();

            query.ExecutionMode = ExecutionMode.Graywulf;
            query.FederationReference.Name = Registry.AppSettings.FederationName;
            query.QueryString = queryString;

            query.SourceDatabaseVersionName = settings.HotDatabaseVersionName;
            query.StatDatabaseVersionName = settings.StatDatabaseVersionName;

            query.QueryTimeout = settings.QueryTimeout;

            // TODO: modify this to allow myscratch, group membership, etc.

            var udf = UserDatabaseFactory.Create(federation);
            var mydbds = udf.GetUserDatabase(user);

            mydbds.IsMutable = true;
            query.CustomDatasets.Add(mydbds);

            query.DefaultDataset = mydbds;

            // Set up MYDB for destination
            // ****** TODO add output table name to settings */
            query.Destination = new DestinationTable(
                mydbds,
                mydbds.DatabaseName,
                settings.DefaultSchemaName,
                String.IsNullOrWhiteSpace(outputTable) ? "outputtable" : outputTable,
                TableInitializationOptions.Drop | TableInitializationOptions.Create);

            // Set up temporary database
            var tempds = new GraywulfDataset(Context);
            tempds.Name = Registry.Constants.TempDbName;
            tempds.IsOnLinkedServer = false;
            tempds.DatabaseVersionReference.Value = federation.TempDatabaseVersion;
            query.TemporaryDataset = tempds;

            // Set up code database
            var codeds = new GraywulfDataset(Context);
            codeds.Name = Registry.Constants.CodeDbName;
            codeds.IsOnLinkedServer = false;
            codeds.DatabaseVersionReference.Value = federation.CodeDatabaseVersion;
            query.CodeDataset = codeds;
        }

        /// <summary>
        /// Initializes a query object for execution outside the Graywulf framework.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="queryString"></param>
        /// <param name="outputTable"></param>
        /// <param name="mydbds"></param>
        /// <param name="tempds"></param>
        /// <param name="codeds"></param>
        protected override void GetInitializedQuery_SingleServer(QueryBase query, string queryString, string outputTable, SqlServerDataset mydbds, SqlServerDataset tempds, SqlServerDataset codeds)
        {
            // TODO: factor it out to a new class

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

        /// <summary>
        /// Scheduler the query as a job.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="query"></param>
        /// <param name="queueName"></param>
        /// <param name="comments"></param>
        /// <returns></returns>
        public override JobInstance ScheduleAsJob(string jobName, QueryBase query, string queueName, string comments)
        {
            var job = CreateJobInstance(
                jobName,
                EntityFactory.CombineName(EntityType.JobDefinition, Registry.AppSettings.FederationName, typeof(SqlQueryJob).Name),
                queueName,
                comments);

            job.Parameters[Constants.JobParameterQuery].Value = query;

            return job;
        }

        /// <summary>
        /// Creates a workflow job that can be used to execute the query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <remarks>
        /// Overrides of this function might return different types of workflows based on
        /// query type. This function is used by the single-server mode command-line utility only.
        /// </remarks>
        public virtual Activity GetAsWorkflow(QueryBase query)
        {
            // TODO: move this to another class

            return new SqlQueryJob();
        }

        /// <summary>
        /// Returns a disctionary of parameters used to configure a query job.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// <remarks>
        /// This function is used by the single-server mode command-line utility only.
        /// </remarks>
        public virtual Dictionary<string, object> GetWorkflowParameters(QueryBase query)
        {
            // TODO: move this to another class

            return new Dictionary<string, object>()
            {
                { Constants.JobParameterQuery, query },
                { Constants.JobParameterUserGuid, Guid.Empty },
                { Constants.JobParameterJobGuid, Guid.Empty },
            };
        }
    }
}
