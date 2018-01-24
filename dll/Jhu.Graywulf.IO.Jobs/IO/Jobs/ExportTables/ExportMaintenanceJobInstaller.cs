using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.IO.Jobs.ExportTables
{
    public class ExportMaintenanceJobInstaller : JobInstallerBase
    {
        protected override Type JobType
        {
            get { return typeof(ExportMaintenanceJob); }
        }

        protected override string DisplayName
        {
            get { return JobNames.ExportMaintenanceJob; }
        }

        protected override bool IsSystem
        {
            get { return true; }
        }

        public ExportMaintenanceJobInstaller(Federation federation)
            : base(federation)
        {
        }

        protected override void CreateSettings(JobDefinition jobDefinition)
        {
            base.CreateSettings(jobDefinition);

            // TODO: implement this
        }
    }
}
