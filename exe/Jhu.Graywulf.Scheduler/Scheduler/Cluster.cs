using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Scheduler
{
    public class Cluster
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

        public Cluster()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        public void Load(string clusterName)
        {
            using (Context context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var ef = new EntityFactory(context);
                var cluster = ef.LoadEntity<Jhu.Graywulf.Registry.Cluster>(clusterName);

                machineRoles = new Dictionary<Guid, MachineRole>();
                machines = new Dictionary<Guid, Machine>();
                serverInstances = new Dictionary<Guid, ServerInstance>();
                databaseInstances = new Dictionary<Guid, DatabaseInstance>();
                databaseDefinitions = new Dictionary<Guid, DatabaseDefinition>();
                queues = new Dictionary<Guid, Queue>();

                cluster.LoadMachineRoles(true);

                // *** TODO: handle machines that are down
                // *** TODO: define root object which limits queues handled by scheduler instance
                foreach (var mr in cluster.MachineRoles.Values)
                {
                    var mri = new MachineRole(mr);
                    machineRoles.Add(mr.Guid, mri);

                    mr.LoadQueueInstances(true);

                    foreach (var qi in mr.QueueInstances.Values)
                    {
                        var q = new Queue();
                        q.Update(qi);
                        queues.Add(qi.Guid, q);
                    }

                    mr.LoadMachines(true);

                    foreach (var mm in mr.Machines.Values)
                    {
                        var mmi = new Machine(mm);
                        machines.Add(mm.Guid, mmi);

                        mm.LoadServerInstances(true);

                        foreach (var si in mm.ServerInstances.Values)
                        {
                            var ssi = new ServerInstance(si);
                            ssi.Machine = mmi;

                            serverInstances.Add(si.Guid, ssi);
                        }

                        mm.LoadQueueInstances(true);

                        foreach (var qi in mm.QueueInstances.Values)
                        {
                            var q = new Queue();
                            q.Update(qi);
                            queues.Add(qi.Guid, q);
                        }
                    }
                }

                cluster.LoadDomains(true);
                foreach (var dom in cluster.Domains.Values)
                {
                    dom.LoadFederations(true);
                    foreach (var ff in dom.Federations.Values)
                    {
                        ff.LoadDatabaseDefinitions(true);
                        foreach (var dd in ff.DatabaseDefinitions.Values)
                        {
                            databaseDefinitions.Add(dd.Guid, new DatabaseDefinition(dd));

                            dd.LoadDatabaseInstances(true);
                            foreach (var di in dd.DatabaseInstances.Values)
                            {
                                var ddi = new DatabaseInstance(di);

                                // add to global list
                                databaseInstances.Add(di.Guid, ddi);

                                // add to database definition lists
                                Dictionary<Guid, DatabaseInstance> databaseinstances;
                                if (databaseDefinitions[dd.Guid].DatabaseInstances.ContainsKey((di.DatabaseVersion.Name)))
                                {
                                    databaseinstances = databaseDefinitions[dd.Guid].DatabaseInstances[di.DatabaseVersion.Name];
                                }
                                else
                                {
                                    databaseinstances = new Dictionary<Guid, DatabaseInstance>();
                                    databaseDefinitions[dd.Guid].DatabaseInstances.Add(di.DatabaseVersion.Name, databaseinstances);
                                }

                                databaseinstances.Add(di.Guid, ddi);

                                ddi.ServerInstance = serverInstances[di.ServerInstanceReference.Guid];
                                ddi.DatabaseDefinition = databaseDefinitions[dd.Guid];
                            }
                        }
                    }
                }
            }
        }

    }
}
