using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Scheduler
{
    internal class Cluster : RegistryObject
    {
        private Dictionary<Guid, MachineRole> machineRoles;

        /// <summary>
        /// Machines available in the cluster
        /// </summary>
        private Dictionary<Guid, Machine> machines;

        /// <summary>
        /// Server instances available to the cluster
        /// </summary>
        private Dictionary<Guid, ServerInstance> serverInstances;

        /// <summary>
        /// Database instances on the cluster
        /// </summary>
        private Dictionary<Guid, DatabaseInstance> databaseInstances;

        /// <summary>
        /// Database definitions configured under the cluster
        /// </summary>
        private Dictionary<Guid, DatabaseDefinition> databaseDefinitions;

        /// <summary>
        /// Queues listed by their registry IDs
        /// </summary>
        /// <remarks>
        /// QueueInstance.Guid, Queue
        /// </remarks>
        private Dictionary<Guid, Queue> queues;

        public Dictionary<Guid, MachineRole> MachineRoles
        {
            get { return machineRoles; }
        }

        public Dictionary<Guid, Machine> Machines
        {
            get { return machines; }
        }

        public Dictionary<Guid, ServerInstance> ServerInstances
        {
            get { return serverInstances; }
        }

        public Dictionary<Guid, DatabaseInstance> DatabaseInstances
        {
            get { return databaseInstances; }
        }

        public Dictionary<Guid, DatabaseDefinition> DatabaseDefinitions
        {
            get { return databaseDefinitions; }
        }

        public Dictionary<Guid, Queue> Queues
        {
            get { return queues; }
        }

        public Cluster(Jhu.Graywulf.Registry.Entity e)
            : base(e)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.machineRoles = new Dictionary<Guid, MachineRole>();
            this.machines = new Dictionary<Guid, Machine>();
            this.serverInstances = new Dictionary<Guid, ServerInstance>();
            this.databaseInstances = new Dictionary<Guid, DatabaseInstance>();
            this.databaseDefinitions = new Dictionary<Guid, DatabaseDefinition>();
            this.queues = new Dictionary<Guid, Queue>();
        }
    }
}
