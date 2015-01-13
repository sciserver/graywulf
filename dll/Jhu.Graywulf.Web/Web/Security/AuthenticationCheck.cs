using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    public class AuthenticationCheck : Jhu.Graywulf.Check.CheckRoutineBase
    {
        public Context Context { get; set; }

        public AuthenticationCheck(Context context)
        {
            this.Context = context;
        }

        public override void Execute(System.IO.TextWriter output)
        {
            output.WriteLine("Checking authentication module.");

            var af = AuthenticationFactory.Create(Context.Domain);

            output.WriteLine("Authentication module: {0}", af.GetType().FullName);
        }

        public override IEnumerable<Graywulf.Check.CheckRoutineBase> GetCheckRoutines()
        {
            var af = AuthenticationFactory.Create(Context.Domain);
            return af.GetCheckRoutines();
        }
    }
}
