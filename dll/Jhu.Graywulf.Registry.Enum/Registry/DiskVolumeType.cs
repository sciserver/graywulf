using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Type of the logical disk volume
    /// </summary>
    [Flags]
    public enum DiskVolumeType : int
    {
        Unknown = 0,
        Spinning = 1,
        SolidState = 2,
        RamDisk = 4,
        Raid = 8
    }
}
