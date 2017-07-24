using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Scheduler
{
    internal class Machine : RegistryObject
    {
        
        public bool IsAvailable
        {
            get { return IsRunning; }
        }

        public Machine(Jhu.Graywulf.Registry.Machine e)
            : base(e)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }
    }
}
