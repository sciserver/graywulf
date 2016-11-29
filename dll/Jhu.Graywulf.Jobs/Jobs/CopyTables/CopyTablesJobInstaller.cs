using System;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Jobs.CopyTables
{
    public class CopyTablesJobInstaller : JobInstallerBase
    {
        public CopyTablesJobInstaller(Federation federation)
            : base(federation)
        {
        }

        protected override Type GetJobType()
        {
            return typeof(Jobs.CopyTables.CopyTablesJob);
        }
    }
}
