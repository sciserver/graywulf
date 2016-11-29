using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install
{
    public abstract class JobInstallerBase : InstallerBase
    {
        private Federation federation;


        protected JobInstallerBase(Federation federation)
            :base(federation.Context)
        {
            this.federation = federation;

        }

        protected abstract Type GetJobType();

        public JobDefinition Install()
        {
            var jd = GenerateJobDefinition(GetJobType());
            jd.Save();

            return jd;
        }

        protected virtual JobDefinition GenerateJobDefinition(Type jobType)
        {
            var jd = new JobDefinition(federation)
            {
                Name = jobType.Name,
                System = federation.System,
                WorkflowTypeName = GetUnversionedTypeName(jobType),
            };
            jd.DiscoverWorkflowParameters();

            return jd;
        }
    }
}
