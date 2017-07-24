using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Scheduler
{
    internal class DatabaseInstance : RegistryObject
    {
        private ServerInstance serverInstance;
        private DatabaseDefinition databaseDefinition;

        public ServerInstance ServerInstance
        {
            get { return serverInstance; }
            set { serverInstance = value; }
        }

        public DatabaseDefinition DatabaseDefinition
        {
            get { return databaseDefinition; }
            set { databaseDefinition = value; }
        }

        public bool IsAttached
        {
            get { return runningState == Registry.RunningState.Attached; }
        }

        public bool IsAvailable
        {
            get { return IsAttached && serverInstance.IsAvailable; }
        }

        public DatabaseInstance(Jhu.Graywulf.Registry.Entity e)
            : base(e)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.serverInstance = null;
            this.databaseDefinition = null;
        }
    }
}
