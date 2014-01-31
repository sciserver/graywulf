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

namespace Jhu.Graywulf.Web.UI.Api
{
    /// <summary>
    /// Wrapper around job factory to support query specific features
    /// </summary>
    public class JobFactory : ContextObject
    {
        #region Static members for entity caching
        
        private static object syncRoot = new object();

        private static bool jobDefinitionsLoaded = false;
        private static ConcurrentDictionary<Guid, string> queryJobDefinitions = null;
        private static ConcurrentDictionary<Guid, string> exportJobDefinitions = null;

        public static HashSet<Guid> QueryJobDefinitionGuids
        {
            get
            {
                lock (syncRoot)
                {
                    return new HashSet<Guid>(queryJobDefinitions.Keys);
                }
            }
        }

        public static HashSet<Guid> ExportJobDefinitionGuids
        {
            get
            {
                lock (syncRoot)
                {
                    return new HashSet<Guid>(exportJobDefinitions.Keys);
                }
            }
        }
        
        #endregion

        private JobInstanceFactory jobFactory;

        public Guid UserGuid
        {
            get { return jobFactory.UserGuid; }
            set { jobFactory.UserGuid = value; }
        }

        public HashSet<Guid> QueueInstanceGuid
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
            LoadJobDefinitions();

            jobFactory = new JobInstanceFactory(Context);
            jobFactory.UserGuid = Context.UserGuid;
        }

        private void LoadJobDefinitions()
        {
            lock (syncRoot)
            {
                if (!jobDefinitionsLoaded)
                {
                    queryJobDefinitions = new ConcurrentDictionary<Guid, string>();
                    exportJobDefinitions = new ConcurrentDictionary<Guid, string>();

                    var ef = new EntityFactory(Context);
                    var f = ef.LoadEntity<Federation>(Jhu.Graywulf.Registry.AppSettings.FederationName);

                    f.LoadJobDefinitions(true);

                    // TODO rewrite this?
                    foreach (var jd in f.JobDefinitions.Values)
                    {
                        var rh = JobReflectionHelper.CreateInstance(jd.WorkflowTypeName);
                        if (rh.HasInterface(typeof(Jhu.Graywulf.Jobs.Query.IQueryJob).ToString()))
                        {
                            queryJobDefinitions.TryAdd(jd.Guid, jd.WorkflowTypeName);
                        }
                        else if (rh.HasInterface(typeof(Jhu.Graywulf.Jobs.ExportTables.IExportTablesJob).ToString()))
                        {
                            exportJobDefinitions.TryAdd(jd.Guid, jd.WorkflowTypeName);
                        }
                    }

                    jobDefinitionsLoaded = true;
                }
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
                var jdguid = jobInstance.JobDefinitionReference.Guid;

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