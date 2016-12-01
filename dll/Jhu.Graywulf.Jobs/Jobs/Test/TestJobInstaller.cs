using System;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Jobs.Test
{
    public class TestJobInstaller : JobInstallerBase
    {
        protected override Type JobType
        {
            get { return typeof(TestJob); }
        }

        protected override string DisplayName
        {
            get { return JobNames.TestJob; }
        }

        protected override bool IsSystem
        {
            get { return true; }
        }

        public TestJobInstaller(Federation federation)
            : base(federation)
        {
        }
    }
}
