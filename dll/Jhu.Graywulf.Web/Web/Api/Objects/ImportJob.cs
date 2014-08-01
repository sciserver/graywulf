using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Api
{
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
