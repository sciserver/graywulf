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

namespace Jhu.Graywulf.Web.Api
{
    /// <summary>
    /// Wrapper around job factory to support query specific features
    /// </summary>
    public class JobFactory : ContextObject
    {
        #region Static members for entity caching

        private static object syncRoot = new object();

        private static bool queueInstancesLoaded = false;
        private static ConcurrentDictionary<string, QueueInstance> queueInstances;

        private static bool jobDefinitionsLoaded = false;
        private static ConcurrentDictionary<string, JobDefinition> queryJobDefinitions;
        private static ConcurrentDictionary<string, JobDefinition> exportJobDefinitions;
        private static ConcurrentDictionary<string, JobDefinition> importJobDefinitions;

        #endregion

        private JobInstanceFactory jobFactory;

        public Guid UserGuid
        {
            get { return jobFactory.UserGuid; }
            set { jobFactory.UserGuid = value; }
        }

        public HashSet<Guid> QueueInstanceGuids
        {
            get { return jobFactory.QueueInstanceGuids; }
        }

        public HashSet<Guid> JobDefinitionGuids
        {
            get { return jobFactory.JobDefinitionGuids; }
        }

        public JobExecutionState JobExecutionStatus
        {
            get { return jobFactory.JobExecutionStatus; }
        }

        public JobFactory(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            LoadQueueInstances();
            LoadJobDefinitions();

            jobFactory = new JobInstanceFactory(Context);
            jobFactory.UserGuid = Context.UserGuid;
        }

        private void LoadQueueInstances()
        {
            // Load only queue instances defined under the controller machine
            // 

            lock (syncRoot)
            {
                if (!queueInstancesLoaded)
                {
                    queueInstances = new ConcurrentDictionary<string, QueueInstance>(Entity.StringComparer);

                    Context.Federation.ControllerMachine.LoadQueueInstances(true);

                    foreach (var q in Context.Federation.ControllerMachine.QueueInstances.Values)
                    {
                        queueInstances.TryAdd(q.Name, q);
                    }

                    queueInstancesLoaded = true;
                }
            }
        }

        private void LoadJobDefinitions()
        {
            lock (syncRoot)
            {
                if (!jobDefinitionsLoaded)
                {
                    queryJobDefinitions = new ConcurrentDictionary<string, JobDefinition>();
                    exportJobDefinitions = new ConcurrentDictionary<string, JobDefinition>();
                    importJobDefinitions = new ConcurrentDictionary<string, JobDefinition>();

                    var ef = new EntityFactory(Context);
                    var f = ef.LoadEntity<Federation>(Jhu.Graywulf.Registry.AppSettings.FederationName);

                    f.LoadJobDefinitions(true);

                    // TODO rewrite this?
                    foreach (var jd in f.JobDefinitions.Values)
                    {
                        var rh = JobReflectionHelper.CreateInstance(jd.WorkflowTypeName);
                        if (rh.HasInterface(typeof(Jhu.Graywulf.Jobs.Query.IQueryJob).ToString()))
                        {
                            queryJobDefinitions.TryAdd(jd.Name, jd);
                        }
                        else if (rh.HasInterface(typeof(Jhu.Graywulf.Jobs.ExportTables.IExportTablesJob).ToString()))
                        {
                            exportJobDefinitions.TryAdd(jd.Name, jd);
                        }
                        // TODO: add more jobs here, especially import!
                    }

                    jobDefinitionsLoaded = true;
                }
            }
        }

        public IEnumerable<QueueInstance> SelectQueueInstances()
        {
            return queueInstances.Values;
        }

        public QueueInstance GetQueueInstance(string name)
        {
            return queueInstances[name];
        }

        public static IEnumerable<JobDefinition> SelectJobDefinitions(JobType type)
        {
            if ((type & JobType.Query) != 0)
            {
                foreach (var jd in queryJobDefinitions.Values)
                {
                    yield return jd;
                }
            }

            if ((type & JobType.Export) != 0)
            {
                foreach (var jd in exportJobDefinitions.Values)
                {
                    yield return jd;
                }
            }

            if ((type & JobType.Import) != 0)
            {
                foreach (var jd in importJobDefinitions.Values)
                {
                    yield return jd;
                }
            }
        }

        public static JobDefinition GetJobDefinition(string name)
        {
            JobDefinition jd;

            if (queryJobDefinitions.TryGetValue(name, out jd))
            {
                return jd;
            }
            else if (exportJobDefinitions.TryGetValue(name, out jd))
            {
                return jd;
            }
            else if (importJobDefinitions.TryGetValue(name, out jd))
            {
                return jd;
            }
            else
            {
                throw new KeyNotFoundException();       // TODO
            }
        }

        public int CountJobs()
        {
            return jobFactory.CountJobInstances();
        }

        public IEnumerable<Job> SelectJobs(int from, int max)
        {
            foreach (var job in jobFactory.FindJobInstances(from, max))
            {
                yield return CreateJobFromInstance(job);
            }
        }

        /// <summary>
        /// Creates a job description from a job instance.
        /// </summary>
        /// <param name="jobInstance"></param>
        /// <returns></returns>
        /// <remarks>
        /// Because we don't want to expose the complex class of JobInstances to the rest api,
        /// we wrap it into simplex classes exposing the necessary properties only.
        /// </remarks>
        public Job CreateJobFromInstance(JobInstance jobInstance)
        {
            Job job;

            try
            {
                var jdguid = jobInstance.JobDefinitionReference.Name;

                if (queryJobDefinitions.ContainsKey(jdguid))
                {
                    return new QueryJob(jobInstance);
                }
                else if (exportJobDefinitions.ContainsKey(jdguid))
                {
                    return new ExportJob(jobInstance);
                }
                else
                {
                    job = new Job(jobInstance);
                }
            }
            catch (Exception)
            {
                job = new Job();
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