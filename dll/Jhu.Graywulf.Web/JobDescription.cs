using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.ExportTable;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Web
{
    /// <summary>
    /// Wrapper class around Jhu.Graywulf.Registry.JobInstance to
    /// support access to query specific job parameters for displaying on the web page
    /// </summary>
    public class JobDescription
    {
        private JobInstance job;
        private JobType jobType;
        private string query;
        private string schemaName;
        private string objectName;
        private string path;

        public JobInstance Job
        {
            get { return job; }
            set { job = value; }
        }

        public JobType JobType
        {
            get { return jobType; }
            set { jobType = value; }
        }

        public Guid Guid
        {
            get { return job.Guid; }
        }

        public string Query
        {
            get { return query; }
            internal set { query = value; }
        }

        public string SchemaName
        {
            get { return schemaName; }
            internal set { schemaName = value; }
        }

        public string ObjectName
        {
            get { return objectName; }
            internal set { objectName = value; }
        }

        public string Path
        {
            get { return path; }
            internal set { path = value; }
        }

        public JobDescription()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.job = null;
            this.jobType = JobType.Unknown;
            this.query = null;
            this.schemaName = null;
            this.objectName = null;
            this.path = null;
        }
    }
}