using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Schema
{
    public class UserDatabaseCheck : Jhu.Graywulf.Check.CheckRoutineBase
    {
        public override CheckCategory Category
        {
            get { return CheckCategory.Database; }
        }

        public FederationContext FederationContext { get; set; }

        public UserDatabaseCheck(FederationContext context)
        {
            this.FederationContext = context;
        }

        protected override IEnumerable<CheckRoutineStatus> OnExecute()
        {
            yield return ReportInfo("Checking user database module.");
            var uf = UserDatabaseFactory.Create(FederationContext);
            yield return ReportSuccess("User database module: {0}", uf.GetType().FullName);
        }

        protected override IEnumerable<Graywulf.Check.CheckRoutineBase> OnGetCheckRoutines()
        {
            var uf = UserDatabaseFactory.Create(FederationContext);
            return uf.GetCheckRoutines();
        }
    }
}
