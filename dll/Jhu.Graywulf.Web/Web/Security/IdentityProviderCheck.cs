using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    public class IdentityProviderCheck : Jhu.Graywulf.Check.CheckRoutineBase
    {
        public Context Context { get; set; }

        public IdentityProviderCheck(Context context)
        {
            this.Context = context;
        }

        public override void Execute(System.IO.TextWriter output)
        {
            output.WriteLine("Checking identity provider module.");

            var ip = IdentityProvider.Create(Context.Domain);
            output.WriteLine("Identity provider module: {0}", ip.GetType().FullName);
        }

        public override IEnumerable<Graywulf.Check.CheckRoutineBase> GetCheckRoutines()
        {
            var ip = IdentityProvider.Create(Context.Domain);
            return ip.GetCheckRoutines();
        }
    }
}
