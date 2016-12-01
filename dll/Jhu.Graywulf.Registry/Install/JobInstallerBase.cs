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

        protected abstract Type JobType { get; }
        protected abstract string DisplayName { get; }
        protected abstract bool IsSystem { get; }

        protected JobInstallerBase(Federation federation)
            :base(federation.Context)
        {
            this.federation = federation;

        }

        public JobDefinition Install()
        {
            var jd = new JobDefinition(federation)
            {
                Name = JobType.Name,
                DisplayName = DisplayName,
                System = federation.System || IsSystem,
                WorkflowTypeName = GetUnversionedTypeName(JobType),
            };

            jd.DiscoverWorkflowParameters();

            CreateSettings(jd);

            jd.Save();

            return jd;
        }

        protected virtual void CreateSettings(JobDefinition jobDefinition)
        {
        }        
    }
}
