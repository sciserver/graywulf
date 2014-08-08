using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Text;
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
        private static ConcurrentDictionary<Guid, JobDefinition> queryJobDefinitionsByGuid;
        private static ConcurrentDictionary<Guid, JobDefinition> exportJobDefinitionsByGuid;
        private static ConcurrentDictionary<Guid, JobDefinition> importJobDefinitionsByGuid;
        private static ConcurrentDictionary<string, JobDefinition> queryJobDefinitionsByName;
        private static ConcurrentDictionary<string, JobDefinition> exportJobDefinitionsByName;
        private static ConcurrentDictionary<string, JobDefinition> importJobDefinitionsByName;

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
                    entityFactory = new EntityFactory(Context);
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
                    // Use user guid from context (only returns jobs of particular user)
                    jobInstanceFactory = new JobInstanceFactory(Context);
                    jobInstanceFactory.UserGuid = Context.UserGuid;
                }

                return jobInstanceFactory;
            }
        }

        public Guid UserGuid
        {
            get { return JobInstanceFactory.UserGuid; }
            set { JobInstanceFactory.UserGuid = value; }
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

        public JobFactory(Context context)
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

            Context.Federation.ControllerMachine.LoadQueueInstances(true);

            foreach (var q in Context.Federation.ControllerMachine.QueueInstances.Values)
            {
                queueInstancesByGuid.TryAdd(q.Guid, q);
                queueInstancesByName.TryAdd(q.Name, q);
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
            queryJobDefinitionsByGuid = new ConcurrentDictionary<Guid, JobDefinition>();
            exportJobDefinitionsByGuid = new ConcurrentDictionary<Guid, JobDefinition>();
            importJobDefinitionsByGuid = new ConcurrentDictionary<Guid, JobDefinition>();
            queryJobDefinitionsByName = new ConcurrentDictionary<string, JobDefinition>();
            exportJobDefinitionsByName = new ConcurrentDictionary<string, JobDefinition>();
            importJobDefinitionsByName = new ConcurrentDictionary<string, JobDefinition>();

            var ef = new EntityFactory(Context);
            var f = ef.LoadEntity<Federation>(Jhu.Graywulf.Registry.AppSettings.FederationName);

            f.LoadJobDefinitions(true);

            // TODO rewrite this?
            foreach (var jd in f.JobDefinitions.Values)
            {
                var rh = JobReflectionHelper.CreateInstance(jd.WorkflowTypeName);
                if (rh.HasInterface(typeof(Jhu.Graywulf.Jobs.Query.IQueryJob).ToString()))
                {
                    queryJobDefinitionsByGuid.TryAdd(jd.Guid, jd);
                    queryJobDefinitionsByName.TryAdd(jd.Name, jd);
                }
                else if (rh.HasInterface(typeof(Jhu.Graywulf.Jobs.ExportTables.IExportTablesJob).ToString()))
                {
                    exportJobDefinitionsByGuid.TryAdd(jd.Guid, jd);
                    exportJobDefinitionsByName.TryAdd(jd.Name, jd);
                }
                // TODO: add more jobs here, especially import!
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

            if (queryJobDefinitionsByName.TryGetValue(name, out jd))
            {
                return jd;
            }
            else if (exportJobDefinitionsByName.TryGetValue(name, out jd))
            {
                return jd;
            }
            else if (importJobDefinitionsByName.TryGetValue(name, out jd))
            {
                return jd;
            }
            else
            {
                throw new KeyNotFoundException();       // TODO
            }
        }

        public IEnumerable<JobDefinition> SelectJobDefinitions(JobType type)
        {
            if ((type & JobType.Query) != 0)
            {
                foreach (var jd in queryJobDefinitionsByName.Values)
                {
                    yield return jd;
                }
            }

            if ((type & JobType.Export) != 0)
            {
                foreach (var jd in exportJobDefinitionsByName.Values)
                {
                    yield return jd;
                }
            }

            if ((type & JobType.Import) != 0)
            {
                foreach (var jd in importJobDefinitionsByName.Values)
                {
                    yield return jd;
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

                if (queryJobDefinitionsByGuid.ContainsKey(jdguid))
                {
                    return new QueryJob(jobInstance);
                }
                else if (exportJobDefinitionsByGuid.ContainsKey(jdguid))
                {
                    return new ExportJob(jobInstance);
                }
                else if (importJobDefinitionsByGuid.ContainsKey(jdguid))
                {
                    return new ImportJob(jobInstance);
                }
                else
                {
                    job = new Job(jobInstance);
                }
            }
            catch (Exception)
            {
                // Something went wrong but at lease we can set the Guid and name
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