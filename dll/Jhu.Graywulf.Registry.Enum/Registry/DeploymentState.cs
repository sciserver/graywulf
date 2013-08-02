using System;
using System.Collections.Generic;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    [Flags]
    public enum DeploymentState
    {
        /*/// <summary>
        /// Default deployment state.
        /// </summary>
        Unknown = 0,

        Pending = 1,
        Changing = 2,

        /// <summary>
        /// Entity is created but not deployed yet or already dropped.
        /// </summary>
        PendingNew = Pending | 4,

        /// <summary>
        /// Entity has been modified and changes are ready to be applied.
        /// </summary>
        PendingModify = Pending | 8,

        /// <summary>
        /// The entity is deployed.
        /// </summary>
        Deployed = 16,

        /// <summary>
        /// The entity is being deployed.
        /// </summary>
        Deploying = Changing | Deployed,

        /// <summary>
        /// The entity (database) is detached.
        /// </summary>
        Detached = 32,

        Undeployed = 64,

        PendingUndeploy = Pending | Undeployed,
        Undeploying = Changing | Undeployed,*/

        Unknown = 0,

        New = 1,

        Deploying = 2,

        Deployed = 4,

        Undeploying = 8,

        Undeployed = 16,
        
    }
}
