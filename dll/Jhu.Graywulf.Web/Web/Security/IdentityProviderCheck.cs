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

        public override void Execute(System.IO.TextWriter output)
        {
            output.WriteLine("Checking identity provider module.");

            var ip = IdentityProvider.Create(FederationContext.Domain);
            output.WriteLine("Identity provider module: {0}", ip.GetType().FullName);
        }

        public override IEnumerable<Graywulf.Check.CheckRoutineBase> GetCheckRoutines()
        {
            var ip = IdentityProvider.Create(FederationContext.Domain);
            return ip.GetCheckRoutines();
        }
    }
}
