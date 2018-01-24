using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.ComponentModel;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Jobs.ExportTables;
using Jhu.Graywulf.Web.UI;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Web.Api.V1
{
    public class SqlScriptJob : Job
    {
        // This job wrapper intentionally doesn't give any details about the
        // script jobs as they are used only internally

        #region Constructors and initializers

        public SqlScriptJob()
        {
            InitializeMembers();
        }

        public static new SqlScriptJob FromJobInstance(JobInstance jobInstance)
        {
            var job = new SqlScriptJob();
            job.LoadFromRegistryObject(jobInstance);

            return job;
        }

        private void InitializeMembers()
        {
            base.Type = JobType.SqlScript;
        }

        #endregion
    }
}
