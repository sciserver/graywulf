using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Install
{
    public class SqlQueryJobInstaller : ContextObject
    {
        private Federation federation;

        public SqlQueryJobInstaller(Federation federation)
            : base(federation.Context)
        {
            this.federation = federation;
        }

        public void Install()
        {
            var jd = new JobDefinition(federation)
            {
                Name = typeof(Jobs.Query.SqlQueryJob).Name,
                System = federation.System,
                WorkflowTypeName = typeof(Jobs.Query.SqlQueryJob).AssemblyQualifiedName,
                Settings = new SqlQueryJobSettings()
                {
                    HotDatabaseVersionName = Registry.Constants.HotDatabaseVersionName,
                    StatDatabaseVersionName = Registry.Constants.StatDatabaseVersionName,
                    DefaultSchemaName = Registry.Constants.DefaultSchemaName,
                    DefaultDatasetName = Registry.Constants.MyDbName,
                    QueryTimeout = 7200,    // TODO
                }
            };
            jd.Save();
        }
    }
}
