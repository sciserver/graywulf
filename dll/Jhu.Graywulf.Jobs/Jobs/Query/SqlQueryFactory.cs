using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Activities;
using Jhu.Graywulf.Parsing;
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

        public SqlQueryFactory(RegistryContext context)
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
        protected override SqlQuery CreateQueryBase(Node root)
        {
            SqlQuery res;
            if (root is SelectStatement)
            {
                res = new SqlQuery(RegistryContext);
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
        protected override void InitializeQuery(SqlQuery query, string queryString)
        {
            var ef = new EntityFactory(RegistryContext);
            var jd = ef.LoadEntity<JobDefinition>(Registry.ContextManager.Configuration.FederationName, typeof(SqlQueryJob).Name);

            var settings = new SqlQueryJobSettings(jd.Settings);

            query.ExecutionMode = ExecutionMode.Graywulf;
            query.FederationReference.Name = Registry.ContextManager.Configuration.FederationName;
            query.QueryString = queryString;

            query.SourceDatabaseVersionName = settings.HotDatabaseVersionName;
            query.StatDatabaseVersionName = settings.StatDatabaseVersionName;

            query.QueryTimeout = settings.QueryTimeout;
        }

        /// <summary>
        /// Scheduler the query as a job.
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="query"></param>
        /// <param name="queueName"></param>
        /// <param name="comments"></param>
        /// <returns></returns>
        public override JobInstance ScheduleAsJob(string jobName, SqlQuery query, string queueName, TimeSpan timeout, string comments)
        {
            var job = CreateJobInstance(
                jobName,
                EntityFactory.CombineName(EntityType.JobDefinition, Registry.ContextManager.Configuration.FederationName, typeof(SqlQueryJob).Name),
                queueName,
                timeout,
                comments);

            job.Parameters[Constants.JobParameterQuery].Value = query;

            return job;
        }
    }
}
