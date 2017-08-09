using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Xml;
using System.Reflection;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Jobs.ExportTables;

namespace Jhu.Graywulf.Web.Api.V1
{
    /// <summary>
    /// Wraps a set of registry functions to simplify job creation
    /// from the web interface and web services.
    /// </summary>
    public class JobFactory : ContextObject
    {
        #region Static members for entity caching

        private static object syncRoot = new object();

        private static bool queueInstancesLoaded = false;
        private static ConcurrentDictionary<Guid, QueueInstance> queueInstancesByGuid;
        private static ConcurrentDictionary<string, QueueInstance> queueInstancesByName;

        private static bool jobDefinitionsLoaded = false;
        private static Dictionary<JobType, ConcurrentDictionary<Guid, JobDefinition>> jobDefinitionsByGuid;
        private static Dictionary<JobType, ConcurrentDictionary<string, JobDefinition>> jobDefinitionsByName;

        #endregion
        #region Private member variables

        private EntityFactory entityFactory;
        private JobInstanceFactory jobInstanceFactory;

        #endregion
        #region Properties

        private EntityFactory EntityFactory
        {
            get
            {
                if (entityFactory == null)
                {
                    entityFactory = new EntityFactory(RegistryContext);
                }

                return entityFactory;
            }
        }

        private JobInstanceFactory JobInstanceFactory
        {
            get
            {
                if (jobInstanceFactory == null)
                {
                    jobInstanceFactory = new JobInstanceFactory(RegistryContext);
                }

                return jobInstanceFactory;
            }
        }

        public HashSet<Guid> QueueInstanceGuids
        {
            get { return JobInstanceFactory.QueueInstanceGuids; }
        }

        public HashSet<Guid> JobDefinitionGuids
        {
            get { return JobInstanceFactory.JobDefinitionGuids; }
        }

        public JobExecutionState JobExecutionStatus
        {
            get { return JobInstanceFactory.JobExecutionStatus; }
        }

        #endregion
        #region Constructors and initializers

        public JobFactory(RegistryContext context)
            : base(context)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            EnsureQueueInstancesLoaded();
            EnsureJobDefinitionsLoaded();
        }

        #endregion

        private void EnsureQueueInstancesLoaded()
        {
            // Load only queue instances defined under the controller machine
            // 

            lock (syncRoot)
            {
                if (!queueInstancesLoaded)
                {
                    LoadQueueInstances();
                }
            }
        }

        private void LoadQueueInstances()
        {
            queueInstancesByGuid = new ConcurrentDictionary<Guid, QueueInstance>();
            queueInstancesByName = new ConcurrentDictionary<string, QueueInstance>(Entity.StringComparer);

            RegistryContext.Federation.ControllerMachineRole.LoadQueueInstances(true);

            foreach (var q in RegistryContext.Federation.ControllerMachineRole.QueueInstances.Values)
            {
                if (!q.System && !q.Hidden)
                {
                    queueInstancesByGuid.TryAdd(q.Guid, q);
                    queueInstancesByName.TryAdd(q.Name, q);
                }
            }

            queueInstancesLoaded = true;
        }

        internal QueueInstance GetQueueInstance(Guid guid)
        {
            EnsureQueueInstancesLoaded();

            return queueInstancesByGuid[guid];
        }

        internal QueueInstance GetQueueInstance(string name)
        {
            return queueInstancesByName[name];
        }

        private void EnsureJobDefinitionsLoaded()
        {
            lock (syncRoot)
            {
                if (!jobDefinitionsLoaded)
                {
                    LoadJobDefinitions();
                }
            }
        }

        private void LoadJobDefinitions()
        {
            jobDefinitionsByGuid = new Dictionary<JobType, ConcurrentDictionary<Guid, JobDefinition>>();
            jobDefinitionsByName = new Dictionary<JobType, ConcurrentDictionary<string, JobDefinition>>();

            foreach (var jobtype in Constants.WellKnownJobInterfaces.Keys)
            {
                jobDefinitionsByGuid[jobtype] = new ConcurrentDictionary<Guid, JobDefinition>();
                jobDefinitionsByName[jobtype] = new ConcurrentDictionary<string, JobDefinition>();
            }

            var ef = new EntityFactory(RegistryContext);
            var f = ef.LoadEntity<Federation>(Jhu.Graywulf.Registry.ContextManager.Configuration.FederationName);

            f.LoadJobDefinitions(true);

            // TODO rewrite this?
            foreach (var jd in f.JobDefinitions.Values)
            {
                var rh = JobReflectionHelper.CreateInstance(jd.WorkflowTypeName);

                foreach (var jobtype in Constants.WellKnownJobInterfaces.Keys)
                {
                    if (rh.HasInterface(Constants.WellKnownJobInterfaces[jobtype].ToString()))
                    {
                        jobDefinitionsByGuid[jobtype].TryAdd(jd.Guid, jd);
                        jobDefinitionsByName[jobtype].TryAdd(jd.Name, jd);
                    }
                }
            }

            jobDefinitionsLoaded = true;
        }

        #region Queue functions

        public Queue GetQueue(string name)
        {
            return new Queue(queueInstancesByName[name]);
        }

        public IEnumerable<Queue> SelectQueue()
        {
            return queueInstancesByName.Values.Select(q => new Queue(q));
        }

        #endregion
        #region Job definition functions

        public JobDefinition GetJobDefinition(string name)
        {
            JobDefinition jd;

            foreach (var jobtype in Constants.WellKnownJobInterfaces.Keys)
            {
                if (jobDefinitionsByName[jobtype].TryGetValue(name, out jd))
                {
                    return jd;
                }
            }

            throw new KeyNotFoundException();       // *** TODO
        }

        public IEnumerable<JobDefinition> SelectJobDefinitions(JobType type)
        {
            foreach (var jobtype in Constants.WellKnownJobInterfaces.Keys)
            {
                if ((type & jobtype) != 0)
                {
                    foreach (var jd in jobDefinitionsByName[jobtype].Values)
                    {
                        yield return jd;
                    }
                }
            }
        }

        #endregion
        #region Job functions

        /// <summary>
        /// Returns a single job. Loads job dependencies.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Job GetJob(Guid guid)
        {
            // Load job and its dependencies
            var ji = EntityFactory.LoadEntity<JobInstance>(guid);
            ji.LoadJobInstancesDependencies(false);

            return CreateJobFromInstance(ji);
        }

        public int CountJobs()
        {
            return JobInstanceFactory.CountJobInstances();
        }

        public int CountPendingJobs(string name)
        {
            var qi = queueInstancesByName[name];

            // We want to count all jobs
            JobInstanceFactory.UserGuid = Guid.Empty;

            // Limit search to the given queue only
            JobInstanceFactory.QueueInstanceGuids.Add(qi.Guid);

            // List pending jobs only
            JobInstanceFactory.JobExecutionStatus = JobExecutionState.AllPending;

            return JobInstanceFactory.CountJobInstances();
        }

        public IEnumerable<Job> SelectJobs(int from, int max)
        {
            foreach (var job in JobInstanceFactory.FindJobInstances(from, max))
            {
                yield return CreateJobFromInstance(job);
            }
        }

        public Job CancelJob(Guid guid)
        {
            var ji = EntityFactory.LoadEntity<JobInstance>(guid);
            ji.Cancel();
            return CreateJobFromInstance(ji);
        }

        #endregion

        /// <summary>
        /// Creates a job description from a job instance.
        /// </summary>
        /// <param name="jobInstance"></param>
        /// <returns></returns>
        /// <remarks>
        /// Because we don't want to expose the complex class of JobInstances to the rest api,
        /// we wrap it into simplex classes exposing the necessary properties only.
        /// </remarks>
        public static Job CreateJobFromInstance(JobInstance jobInstance)
        {
            Job job;

            try
            {
                // Use guid here to save on registry access
                var jdguid = jobInstance.JobDefinitionReference.Guid;

                foreach (var jobtype in Constants.WellKnownJobInterfaces.Keys)
                {
                    if (jobDefinitionsByGuid[jobtype].ContainsKey(jdguid))
                    {
                        var type = Constants.WellKnownJobTypes[jobtype];
                        var sm = type.GetMethod("FromJobInstance", BindingFlags.Public | BindingFlags.Static);
                        job = (Job)sm.Invoke(null, new[] { jobInstance });
                        return job;
                    }
                }

                job = Job.FromJobInstance(jobInstance);
            }
            catch (Exception)
            {
                // Something went wrong but at least we can set the Guid and name
                job = new Job()
                {
                    Guid = jobInstance.Guid,
                    Name = jobInstance.Name,
                };
            }

            return job;
        }

        private static string GetXmlInnerText(XmlDocument xml, string path)
        {
            return GetXmlInnerText(xml.ChildNodes, path.Split('/'), 0);
        }

        private static string GetXmlInnerText(XmlNodeList nodes, string[] path, int i)
        {
            for (int k = 0; k < nodes.Count; k++)
            {
                var n = nodes[k];
                if (StringComparer.InvariantCultureIgnoreCase.Compare(n.LocalName, path[i]) == 0)
                {
                    if (i == path.Length - 1)
                    {
                        return n.InnerText;
                    }
                    else
                    {
                        return GetXmlInnerText(n.ChildNodes, path, i + 1);
                    }
                }
            }

            throw new KeyNotFoundException();
        }
    }
}