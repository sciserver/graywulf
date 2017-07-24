using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Scheduler
{
    internal class ServerInstance : RegistryObject
    {
        private Machine machine;
        private DateTime lastAssigned;

        public Machine Machine
        {
            get { return machine; }
            set { machine = value; }
        }

        public DateTime LastAssigned
        {
            get { return lastAssigned; }
            set { lastAssigned = value; }
        }

        public bool IsAvailable
        {
            get { return Machine.IsAvailable; }
        }

        public ServerInstance(Jhu.Graywulf.Registry.ServerInstance e)
            :base(e)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.machine = null;
            this.lastAssigned = DateTime.MinValue;
        }

    }
}
