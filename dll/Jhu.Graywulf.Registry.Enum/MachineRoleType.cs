using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Type of the machine role
    /// </summary>
    public enum MachineRoleType : int
    {
        /// <summary>
        /// Default machine role type
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// Machines in the role are stand alone machines
        /// </summary>
        StandAlone = 1,

        /// <summary>
        /// Machines in the role form a failover set with identical configuration
        /// </summary>
        FailoverSet = 2,

        /// <summary>
        /// Machines in the role contain mirrored data
        /// </summary>
        MirroredSet = 3,

        /// <summary>
        /// Machines in the role conrain chunks of a large dataset
        /// </summary>
        SlicedSet = 4
    }
}
