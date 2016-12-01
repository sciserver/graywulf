using System;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Jobs.MirrorDatabase
{
    public class MirrorDatabaseJobInstaller : JobInstallerBase
    {
        protected override Type JobType
        {
            get { return typeof(MirrorDatabaseJob); }
        }

        protected override string DisplayName
        {
            get { return JobNames.MirrorDatabaseJob; }
        }

        protected override bool IsSystem
        {
            get { return true; }
        }

        public MirrorDatabaseJobInstaller(Federation federation)
            : base(federation)
        {
        }
    }
}
