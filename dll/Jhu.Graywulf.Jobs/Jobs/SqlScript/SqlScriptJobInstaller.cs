using System;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Jobs.SqlScript
{
    public class SqlScriptJobInstaller : JobInstallerBase
    {
        public SqlScriptJobInstaller(Federation federation)
            : base(federation)
        {
        }

        protected override Type GetJobType()
        {
            return typeof(Jobs.SqlScript.SqlScriptJob);
        }
    }
}
