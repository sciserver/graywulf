using System;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Jobs.SqlScript
{
    public class SqlScriptJobInstaller : InstallerBase
    {
        private Federation federation;

        public SqlScriptJobInstaller(Federation federation)
            : base(federation.Context)
        {
            this.federation = federation;
        }

        public virtual JobDefinition Install()
        {
            return GenerateJobDefinition(typeof(Jobs.SqlScript.SqlScriptJob));
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
            jd.Save();

            return jd;
        }
    }
}
