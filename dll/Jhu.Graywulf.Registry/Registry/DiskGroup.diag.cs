using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Jhu.Graywulf.Registry
{
    public partial class DiskGroup
    {
        public override IList<DiagnosticMessage> RunDiagnostics()
        {
            List<DiagnosticMessage> msg = new List<DiagnosticMessage>();

            LoadDiskVolumes(false);
            foreach (DiskVolume dv in this.DiskVolumes.Values)
            {
                msg.AddRange(dv.RunDiagnostics());
            }

            return msg;
        }
    }
}
