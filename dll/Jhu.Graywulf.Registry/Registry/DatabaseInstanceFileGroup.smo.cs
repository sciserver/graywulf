using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using smo = Microsoft.SqlServer.Management.Smo;

namespace Jhu.Graywulf.Registry
{
    public partial class DatabaseInstanceFileGroup
    {
        internal smo::FileGroup GetSmoFileGroup()
        {
            return DatabaseInstance.GetSmoDatabase().FileGroups[FileGroupName];
        }

        internal smo::Database GetSmoDatabase()
        {
            return DatabaseInstance.GetSmoDatabase();
        }

        internal void LoadFromSmo(smo::FileGroup smofg)
        {
            this.Name = smofg.Name;
            this.FileGroupName = smofg.Name;
            this.FileGroupType = FileGroupType.Data;
            // this.PartitionReference        // TODO: how to discover it automatically?

            this.DeploymentState = Registry.DeploymentState.Deployed;
            this.RunningState = Registry.RunningState.Running;

            this.AllocatedSpace = 0;
            this.UsedSpace = 0;
            this.ReservedSpace = 0;
            foreach (var smofile in smofg.Files.Cast<smo::DataFile>())
            {
                this.AllocatedSpace += (long)Math.Ceiling(smofile.Size * 0x400L);     // given in KB, bug in docs!
                this.UsedSpace += (long)Math.Ceiling(smofile.UsedSpace * 0x400L);
                this.ReservedSpace += smofile.MaxSize == -1 ? 0L : (long)Math.Ceiling(smofile.MaxSize * 0x400L);
            }
        }

        internal void LoadFromSmo(smo::Database smodb)
        {
            this.Name = "LOG";
            this.FileGroupName = "LOG";
            this.FileGroupType = FileGroupType.Log;
            this.PartitionReference.Guid = Guid.Empty;

            this.DeploymentState = Registry.DeploymentState.Deployed;
            this.RunningState = Registry.RunningState.Running;

            // Calculate log size
            this.AllocatedSpace = 0;
            this.UsedSpace = 0;
            this.ReservedSpace = 0;
            foreach (var smolf in smodb.LogFiles.Cast<smo::LogFile>())
            {
                this.AllocatedSpace += (long)Math.Ceiling(smolf.Size * 0x400L);    // given in KB, bug in docs!
                this.UsedSpace += (long)Math.Ceiling(smolf.UsedSpace * 0x400L);
                this.ReservedSpace += smolf.MaxSize == -1 ? 0L : (long)Math.Ceiling(smolf.MaxSize * 0x400L);
            }
        }
    }
}
