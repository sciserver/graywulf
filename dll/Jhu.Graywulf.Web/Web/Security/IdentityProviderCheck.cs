using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    public class IdentityProviderCheck : Jhu.Graywulf.Check.CheckRoutineBase
    {
        public override CheckCategory Category
        {
            get { return CheckCategory.Plugin; }
        }

        public FederationContext FederationContext { get; set; }

        public IdentityProviderCheck(FederationContext context)
        {
            this.FederationContext = context;
        }

        protected override IEnumerable<CheckRoutineStatus> OnExecute()
        {
            yield return ReportInfo("Checking identity provider module.");
            var ip = IdentityProvider.Create(FederationContext.Domain);
            yield return ReportSuccess("Identity provider module: {0}", ip.GetType().FullName);
        }

        protected override IEnumerable<Graywulf.Check.CheckRoutineBase> OnGetCheckRoutines()
        {
            var ip = IdentityProvider.Create(FederationContext.Domain);
            return ip.GetCheckRoutines();
        }
    }
}
