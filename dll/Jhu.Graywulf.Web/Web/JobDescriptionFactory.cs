using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Text;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Jobs.ExportTable;

namespace Jhu.Graywulf.Web
{
    /// <summary>
    /// Wrapper around job factory to support query specific features
    /// </summary>
    public class JobDescriptionFactory : ContextObject
    {
        private static object syncRoot = new object();

        private static bool jobDefinitionsLoaded = false;
        private static Dictionary<Guid, string> queryJobDefinitions = null;
        private static Dictionary<Guid, string> exportJobDefinitions = null;

        private WebJobInstanceFactory jobFactory;
        private QueryFactory queryFactory;

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

        public Guid UserGuid
        {
            get { return jobFactory.UserGuid; }
            set { jobFactory.UserGuid = value; }
        }

        public Guid QueueInstanceGuid
        {
            get { return jobFactory.QueueInstanceGuid; }
            set { jobFactory.QueueInstanceGuid = value; }
        }

        public HashSet<Guid> JobDefinitionGuids
        {
            get { return jobFactory.JobDefinitionGuids; }
        }

        public JobExecutionState JobExecutionStatus
        {
            get { return jobFactory.JobExecutionStatus; }
        }

        public JobDescriptionFactory(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        public JobDescriptionFactory(Context context, QueryFactory queryFactory)
            :base(context)
        {
            InitializeMembers();

            this.queryFactory = queryFactory;
        }

        private void InitializeMembers()
        {
            LoadJobDefinitions();

            jobFactory = new WebJobInstanceFactory(Context);
            jobFactory.UserGuid = Context.UserGuid;
        }


        private void LoadJobDefinitions()
        {
            lock (syncRoot)
            {
                if (!jobDefinitionsLoaded)
                {
                    queryJobDefinitions = new Dictionary<Guid, string>();
                    exportJobDefinitions = new Dictionary<Guid, string>();

                    var ef = new EntityFactory(Context);
                    var f = ef.LoadEntity<Federation>(Federation.AppSettings.FederationName);

                    f.LoadJobDefinitions(true);

                    

                    // TODO rewrite this?
                    foreach (var jd in f.JobDefinitions.Values)
                    {
                        var rh = JobReflectionHelper.CreateInstance(jd.WorkflowTypeName);
                        if (rh.HasInterface(typeof(Jhu.Graywulf.Jobs.Query.IQueryJob).ToString()))
                        {
                            queryJobDefinitions.Add(jd.Guid, jd.WorkflowTypeName);
                        }
                        else if (rh.HasInterface(typeof(Jhu.Graywulf.Jobs.ExportTable.IExportJob).ToString()))
                        {
                            exportJobDefinitions.Add(jd.Guid, jd.WorkflowTypeName);
                        }
                    }
                }
            }
        }

        public int CountJobs()
        {
            return jobFactory.CountChildren();
        }

        public IEnumerable<JobDescription> SelectJobs(int from, int max)
        {
            foreach (JobInstance job in jobFactory.SelectChildren(from, max))
            {
                job.LoadParameters();
                yield return GetJob(job, queryFactory);
            }
        }

        public static JobDescription GetJob(JobInstance job, QueryFactory queryFactory)
        {
            // In this function, we don't directly deserialize query parameters because
            // that could break old job definitions once the job format changes. It's
            // save to read parameters from the xml representation directly.

            JobDescription res = new JobDescription();
            res.Job = job;

            if (queryJobDefinitions.ContainsKey(job.JobDefinitionReference.Guid))
            {
                res.JobType = JobType.Query;

                var xml = new XmlDocument();
                xml.LoadXml(job.Parameters["Query"].XmlValue);

                res.Query = GetXmlInnerText(xml, "Query/QueryString");
                res.SchemaName = GetXmlInnerText(xml, "Query/Destination/Table/SchemaName");
                res.ObjectName = GetXmlInnerText(xml, "Query/Destination/Table/ObjectName");
            }
            else if (exportJobDefinitions.ContainsKey(job.JobDefinitionReference.Guid))
            {
                res.JobType = JobType.ExportTable;

                var xml = new XmlDocument();
                xml.LoadXml(job.Parameters["Parameters"].XmlValue);

                res.SchemaName = GetXmlInnerText(xml, "ExportTable/Source/SchemaName");
                res.ObjectName = GetXmlInnerText(xml, "ExportTable/Source/ObjectName");
                res.Path = GetXmlInnerText(xml, "ExportTable/Destination/Path");
            }
            else
            {
                res.JobType = JobType.Unknown;
            }

            return res;
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