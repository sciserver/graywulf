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
        protected override void InitializeQuery(QueryBase query, string queryString)
        {
            var ef = new EntityFactory(Context);
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

            var udf = UserDatabaseFactory.Create(Federation);
            var mydbds = udf.GetUserDatabase(user);

            mydbds.IsMutable = true;
            query.CustomDatasets.Add(mydbds);
            query.DefaultDataset = mydbds;

            // Set up temporary database
            var tempds = new GraywulfDataset(Context);
            tempds.Name = Registry.Constants.TempDbName;
            tempds.IsOnLinkedServer = false;
            tempds.DatabaseVersionReference.Value = Federation.TempDatabaseVersion;
            query.TemporaryDataset = tempds;

            // Set up code database
            var codeds = new GraywulfDataset(Context);
            codeds.Name = Registry.Constants.CodeDbName;
            codeds.IsOnLinkedServer = false;
            codeds.DatabaseVersionReference.Value = Federation.CodeDatabaseVersion;
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
    }
}
