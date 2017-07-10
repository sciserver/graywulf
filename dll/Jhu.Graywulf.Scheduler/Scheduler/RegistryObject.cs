using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gw = Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Scheduler
{
    internal class RegistryObject
    {
        private Guid guid;
        private string name;
        private string fullyQualifiedName;
        protected gw::RunningState runningState;

        public Guid Guid
        {
            get { return guid; }
        }

        public string Name
        {
            get { return name; }
        }

        public string FullyQualifiedName
        {
            get { return fullyQualifiedName; }
        }

        public bool IsRunning
        {
            get { return runningState == Registry.RunningState.Running; }
        }

        public RegistryObject(gw::Entity e)
        {
            InitializeMembers();

            guid = e.Guid;
            name = e.Name;
            fullyQualifiedName = e.GetFullyQualifiedName();
            runningState = e.RunningState;
        }

        private void InitializeMembers()
        {
            this.guid = Guid.Empty;
            this.name = null;
            this.fullyQualifiedName = null;
            this.runningState = Registry.RunningState.Unknown;
        }
    }
}
