using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    public partial class Entity
    {
        public virtual void Deploy()
        {
            this.deploymentState = Registry.DeploymentState.Deployed;
            Save();
        }

        public virtual void Undeploy()
        {
            this.deploymentState = Registry.DeploymentState.Undeployed;
            Save();
        }
    }
}
