using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    public class AuthenticationCheck : Jhu.Graywulf.Check.CheckRoutineBase
    {
        public override CheckCategory Category
        {
            get { return CheckCategory.Plugin; }
        }

        public RegistryContext Context { get; set; }

        public AuthenticationCheck(RegistryContext context)
        {
            this.Context = context;
        }

        protected override IEnumerable<CheckRoutineStatus> OnExecute()
        {
            yield return ReportInfo("Checking authentication module.");
            var af = AuthenticationFactory.Create(Context.Domain);
            yield return ReportSuccess("Authentication module: {0}", af.GetType().FullName);
        }

        protected override IEnumerable<Graywulf.Check.CheckRoutineBase> OnGetCheckRoutines()
        {
            var af = AuthenticationFactory.Create(Context.Domain);
            return af.GetCheckRoutines();
        }
    }
}
