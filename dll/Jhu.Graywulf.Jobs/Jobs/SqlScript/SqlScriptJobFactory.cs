using System;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Jobs.SqlScript
{
    public class SqlScriptJobFactory : JobFactoryBase
    {
        #region Static members

        public static SqlScriptJobFactory Create(RegistryContext context)
        {
            return new SqlScriptJobFactory(context);
        }

        #endregion
        #region Constructors and initializer

        protected SqlScriptJobFactory()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        protected SqlScriptJobFactory(RegistryContext context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        #endregion

        public SqlScriptParameters CreateParameters(DatasetBase[] datasets, string query)
        {
            return new SqlScriptParameters()
            {
                Datasets = datasets,
                Script = query
            };
        }

        public JobInstance ScheduleAsJob(SqlScriptParameters parameters, string queueName, TimeSpan timeout, string comments)
        {
            var job = CreateJobInstance(null, GetJobDefinitionName(), queueName, timeout, comments);
            job.Parameters[Constants.JobParameterSqlScript].Value = parameters;
            return job;
        }

        private string GetJobDefinitionName()
        {
            return EntityFactory.CombineName(EntityType.JobDefinition, RegistryContext.Cluster.Name, RegistryContext.Domain.Name, RegistryContext.Federation.Name, typeof(SqlScriptJob).Name);
        }
    }
}
