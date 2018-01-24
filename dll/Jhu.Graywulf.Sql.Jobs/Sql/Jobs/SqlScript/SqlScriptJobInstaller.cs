using System;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Sql.Jobs.SqlScript
{
    public class SqlScriptJobInstaller : JobInstallerBase
    {
        protected override Type JobType
        {
            get { return typeof(Jobs.SqlScript.SqlScriptJob); }
        }

        protected override string DisplayName
        {
            get { return JobNames.SqlScriptJob; }
        }

        protected override bool IsSystem
        {
            get { return true; }
        }

        public SqlScriptJobInstaller(Federation federation)
            : base(federation)
        {
        }
    }
}
