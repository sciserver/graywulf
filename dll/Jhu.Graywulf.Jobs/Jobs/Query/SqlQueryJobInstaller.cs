using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Jobs.Query
{
    public class SqlQueryJobInstaller : JobInstallerBase
    {
        protected override Type JobType
        {
            get { return typeof(SqlQueryJob); }
        }

        protected override string DisplayName
        {
            get { return JobNames.SqlQueryJob; }
        }

        protected override bool IsSystem
        {
            get { return false; }
        }

        public SqlQueryJobInstaller(Federation federation)
            : base(federation)
        {
        }

        protected override void CreateSettings(JobDefinition jobDefinition)
        {
            base.CreateSettings(jobDefinition);

            jobDefinition.Settings = new SqlQueryJobSettings()
            {
                HotDatabaseVersionName = Registry.Constants.ProdDatabaseVersionName,
                StatDatabaseVersionName = Registry.Constants.StatDatabaseVersionName,
                DefaultSchemaName = Schema.SqlServer.Constants.DefaultSchemaName,
                DefaultDatasetName = Registry.Constants.UserDbName,
                QueryTimeout = 7200,    // TODO
            };
        }
    }
}
