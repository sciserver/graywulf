using System;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.IO.Jobs.CopyTables
{
    public class CopyTablesJobInstaller : JobInstallerBase
    {
        public CopyTablesJobInstaller(Federation federation)
            : base(federation)
        {
        }

        protected override Type JobType
        {
            get { return typeof(Jobs.CopyTables.CopyTablesJob); }
        }

        protected override string DisplayName
        {
            get { return JobNames.CopyTablesJob; }
        }

        protected override bool IsSystem
        {
            get { return false; }
        }
    }
}
