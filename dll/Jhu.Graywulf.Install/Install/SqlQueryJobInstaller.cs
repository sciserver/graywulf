using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Install
{
    public class SqlQueryJobInstaller : InstallerBase
    {
        private Federation federation;

        public SqlQueryJobInstaller(Federation federation)
            : base(federation.Context)
        {
            this.federation = federation;
        }

        public virtual JobDefinition Install()
        {
            return GenerateJobDefinition(typeof(Jobs.Query.SqlQueryJob));
        }

        protected virtual JobDefinition GenerateJobDefinition(Type jobType)
        {
            var jd = new JobDefinition(federation)
            {
                Name = jobType.Name,
                System = federation.System,
                WorkflowTypeName = GetUnversionedTypeName(jobType),
                Settings = new SqlQueryJobSettings()
                {
                    HotDatabaseVersionName = Registry.Constants.ProdDatabaseVersionName,
                    StatDatabaseVersionName = Registry.Constants.StatDatabaseVersionName,
                    DefaultSchemaName = Schema.SqlServer.Constants.DefaultSchemaName,
                    DefaultDatasetName = Registry.Constants.UserDbName,
                    QueryTimeout = 7200,    // TODO
                }
            };
            jd.DiscoverWorkflowParameters();
            jd.Save();

            return jd;
        }
    }
}
