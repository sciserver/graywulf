using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using System.Activities;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Sql.Jobs.Query
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
        protected override SqlQuery OnCreateQuery(Graywulf.Sql.Parsing.StatementBlock parsingTree)
        {
            SqlQuery q;

            if (parsingTree is StatementBlock)
            {
                q = new SqlQuery(null, RegistryContext);
            }
            else
            {
                throw new NotImplementedException();
            }

            return q;
        }

        /// <summary>
        /// Creates a parser object that produces a parsing tree this class can handle.
        /// </summary>
        /// <returns></returns>
        public override Jhu.Graywulf.Parsing.Parser CreateParser()
        {
            return new Sql.Extensions.Parsing.GraywulfSqlParser();
        }

        /// <summary>
        /// Creates a validator object that can validate queries this class can handle.
        /// </summary>
        /// <returns></returns>
        public override Sql.Validation.SqlValidator CreateValidator()
        {
            return new Sql.Validation.SqlValidator();
        }

        /// <summary>
        /// Creates a name resolver object that can resolve identifiers in queirs
        /// this class can handle.
        /// </summary>
        /// <returns></returns>
        public override Sql.NameResolution.SqlNameResolver CreateNameResolver()
        {
            return new Sql.Extensions.NameResolution.GraywulfSqlNameResolver();
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
        protected override void OnSetParameters(SqlQuery query)
        {
            var ef = new EntityFactory(RegistryContext);
            var jd = ef.LoadEntity<JobDefinition>(Registry.ContextManager.Configuration.FederationName, typeof(SqlQueryJob).Name);

            var settings = new SqlQueryJobSettings(jd.Settings);

            query.Parameters.ExecutionMode = ExecutionMode.Graywulf;
            query.Parameters.FederationName = Registry.ContextManager.Configuration.FederationName;

            query.Parameters.SourceDatabaseVersionName = settings.HotDatabaseVersionName;
            query.Parameters.StatDatabaseVersionName = settings.StatDatabaseVersionName;

            query.Parameters.QueryTimeout = settings.QueryTimeout;
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

            job.Parameters[Registry.Constants.JobParameterParameters].Value = query.Parameters;

            return job;
        }
    }
}
