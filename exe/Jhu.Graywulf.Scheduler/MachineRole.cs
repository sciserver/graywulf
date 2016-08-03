using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Scheduler
{
    public class MachineRole : RegistryObject
    {
        public bool IsAvailable
        {
            get { return IsRunning; }
        }

        public MachineRole(Jhu.Graywulf.Registry.MachineRole e)
            : base(e)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }
    }
}
