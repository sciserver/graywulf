using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities.Hosting;
using System.Configuration;
using Jhu.Graywulf.Logging;
using gw = Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Scheduler
{
    /// <summary>
    /// Implements logic to direct requests to a specific server
    /// </summary>
    public class Scheduler : MarshalByRefObject, IScheduler
    {
        public static SchedulerConfiguration Configuration
        {
            get
            {
                return (SchedulerConfiguration)ConfigurationManager.GetSection("jhu.graywulf/scheduler");
            }
        }

        private object syncRoot;
        private QueueManager queueManager;

        internal Scheduler(QueueManager queueManager)
        {
            InitializeMembers();

            this.queueManager = queueManager;
        }

        private void InitializeMembers()
        {
            this.syncRoot = new object();
            this.queueManager = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This is required to prevent unloading the instance referenced by remoting clients only.
        /// </remarks>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Extract information from a running job to set the parameters of a registry context.
        /// </summary>
        /// <param name="workflowInstanceId"></param>
        /// <param name="userGuid"></param>
        /// <param name="userName"></param>
        /// <param name="jobGuid"></param>
        /// <param name="jobID"></param>
        public JobInfo GetJobInfo(Guid workflowInstanceId)
        {
            return queueManager.GetJobInto(workflowInstanceId);
        }

        public Guid GetNextServerInstance(Guid[] databaseDefinitions, string databaseVersion, Guid[] databaseInstances)
        {
            var sis = GetServerInstancesInternal(databaseDefinitions, databaseVersion, databaseInstances);

            if (sis.Length > 0)
            {
                return sis[GetNextServerIndex(sis)].Guid;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public Guid[] GetServerInstances(Guid[] databaseDefinitions, string databaseVersion, Guid[] databaseInstances)
        {
            return GetServerInstancesInternal(databaseDefinitions, databaseVersion, databaseInstances).Select(x => x.Guid).ToArray();
        }

        public Guid GetNextDatabaseInstance(Guid databaseDefinition, string databaseVersion)
        {
            if (!queueManager.Cluster.DatabaseDefinitions.ContainsKey(databaseDefinition))
            {
                return Guid.Empty;
            }

            if (!queueManager.Cluster.DatabaseDefinitions[databaseDefinition].DatabaseInstances.ContainsKey(databaseVersion))
            {
                return Guid.Empty;
            }

            var q =
                from di in queueManager.Cluster.DatabaseDefinitions[databaseDefinition].DatabaseInstances[databaseVersion].Values
                where di.ServerInstance.IsAvailable
                select di;

            var dis = q.ToArray();

            var sis = new ServerInstance[dis.Length];
            for (int i = 0; i < sis.Length; i++)
            {
                sis[i] = dis[i].ServerInstance;
            }

            if (sis.Length > 0)
            {
                return sis[GetNextServerIndex(sis)].Guid;
            }
            else
            {
                return Guid.Empty;
            }
        }

        public Guid[] GetDatabaseInstances(Guid databaseDefinition, string databaseVersion)
        {
            if (!queueManager.Cluster.DatabaseDefinitions.ContainsKey(databaseDefinition))
            {
                return null;
            }

            if (!queueManager.Cluster.DatabaseDefinitions[databaseDefinition].DatabaseInstances.ContainsKey(databaseVersion))
            {
                return null;
            }

            return queueManager.Cluster.DatabaseDefinitions[databaseDefinition].DatabaseInstances[databaseVersion].Keys.ToArray();
        }

        public Guid[] GetDatabaseInstances(Guid serverInstance, Guid databaseDefinition, string databaseVersion)
        {
            if (!queueManager.Cluster.DatabaseDefinitions.ContainsKey(databaseDefinition))
            {
                return null;
            }

            if (!queueManager.Cluster.DatabaseDefinitions[databaseDefinition].DatabaseInstances.ContainsKey(databaseVersion))
            {
                return null;
            }

            var q = from di in queueManager.Cluster.DatabaseDefinitions[databaseDefinition].DatabaseInstances[databaseVersion].Values
                    where di.ServerInstance.IsAvailable && di.ServerInstance.Guid == serverInstance
                    select di.Guid;

            return q.ToArray();
        }

        private int GetNextServerIndex(ServerInstance[] serverInstances)
        {
            lock (syncRoot)
            {
                // Find server with the earliest time stamp
                DateTime min = DateTime.MaxValue;
                int m = -1;

                for (int i = 0; i < serverInstances.Length; i++)
                {
                    ServerInstance si = serverInstances[i];

                    if (si.LastAssigned < min)
                    {
                        min = si.LastAssigned;
                        m = i;
                    }
                }

                if (m == -1)
                {
                    throw new SchedulerException(ExceptionMessages.NoServerForDatabaseFound);
                }

                serverInstances[m].LastAssigned = DateTime.Now;

                return m;
            }
        }

        /// <summary>
        /// Returns the server instances that all contain an instance
        /// the database definition
        /// </summary>
        /// <param name="databaseDefinitions"></param>
        /// <param name="databaseVersionName"></param>
        /// <returns></returns>
        private ServerInstance[] GetServerInstancesInternal(Guid[] databaseDefinitions, string databaseVersionName, Guid[] databaseInstances)
        {
            HashSet<ServerInstance> sis;

            if (databaseDefinitions != null && databaseDefinitions.Length > 0)
            {
                sis = new HashSet<ServerInstance>(GetServerInstancesInternal(databaseDefinitions, databaseVersionName));
            }
            else
            {
                sis = new HashSet<ServerInstance>(GetAllAvailableServerInstances());
            }

            // If there's any database instances specified then a specific server instance will be required
            if (databaseInstances != null && databaseInstances.Length > 0)
            {
                var disis = new HashSet<ServerInstance>();
                foreach (var di in databaseInstances)
                {
                    var si = queueManager.Cluster.DatabaseInstances[di].ServerInstance;
                    if (si.IsAvailable)
                    {
                        disis.Add(si);
                    }
                }

                sis.IntersectWith(disis);
            }

            return sis.ToArray();
        }

        private ServerInstance[] GetServerInstancesInternal(Guid[] databaseDefinitions, string databaseVersionName)
        {
            var sis = new HashSet<ServerInstance>();

            foreach (var dd in databaseDefinitions)
            {
                if (queueManager.Cluster.DatabaseDefinitions[dd].DatabaseInstances.ContainsKey(databaseVersionName))
                {
                    foreach (var di in queueManager.Cluster.DatabaseDefinitions[dd].DatabaseInstances[databaseVersionName].Values)
                    {
                        var si = di.ServerInstance;

                        if (si.IsAvailable && di.IsAvailable)
                        {
                            sis.Add(si);
                        }
                    }
                }
            }

            return sis.ToArray();
        }

        private ServerInstance[] GetAllAvailableServerInstances()
        {
            return queueManager.Cluster.ServerInstances.Values.Where(i => i.IsAvailable).ToArray();
        }
    }
}
