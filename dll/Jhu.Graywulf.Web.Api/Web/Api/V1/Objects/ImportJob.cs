using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Api.V1
{
    [Description("Represents a data table import job.")]
    public class ImportJob : Job
    {
        public ImportJob()
        {

        }

        public ImportJob(JobInstance jobInstance)
            : base(jobInstance)
        {
        }
    }
}
