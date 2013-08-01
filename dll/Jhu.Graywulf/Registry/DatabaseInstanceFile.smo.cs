using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using smo = Microsoft.SqlServer.Management.Smo;

namespace Jhu.Graywulf.Registry
{
    public partial class DatabaseInstanceFile
    {
        internal smo::DataFile GetSmoFile()
        {
            return DatabaseInstanceFileGroup.GetSmoFileGroup().Files[Name];
        }

        internal smo::LogFile GetSmoLogFile()
        {
            return DatabaseInstanceFileGroup.GetSmoDatabase().LogFiles[Name];
        }

        internal void LoadFromSmo(smo::DataFile smofile)
        {
            this.Name = smofile.Name;
            this.LogicalName = smofile.Name;
            this.Filename = smofile.FileName;
            this.DatabaseFileType = DatabaseFileType.Data;

            this.DeploymentState = Registry.DeploymentState.Deployed;
            this.RunningState = Registry.RunningState.Running;

            this.AllocatedSpace = (long)Math.Ceiling(smofile.Size * 0x400L);     // given in KB, bug in docs!
            this.UsedSpace = (long)(smofile.UsedSpace * 0x400L);
            this.ReservedSpace = smofile.MaxSize == -1 ? 0L : (long)(smofile.MaxSize * 0x400L);
        }

        internal void LoadFromSmo(smo::LogFile smolf)
        {
            this.Name = smolf.Name;
            this.LogicalName = smolf.Name;
            this.Filename = smolf.FileName;
            this.DatabaseFileType = DatabaseFileType.Log;

            this.DeploymentState = Registry.DeploymentState.Deployed;
            this.RunningState = Registry.RunningState.Running;

            this.AllocatedSpace = (long)Math.Ceiling(smolf.Size * 0x400L);     // given in KB, bug in docs!
            this.UsedSpace = (long)(smolf.UsedSpace * 0x400L);
            this.ReservedSpace = smolf.MaxSize == -1 ? 0L : (long)(smolf.MaxSize * 0x400L);
        }
    }
}
