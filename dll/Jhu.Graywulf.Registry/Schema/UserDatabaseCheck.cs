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

        public override void Execute(System.IO.TextWriter output)
        {
            output.WriteLine("Checking user database module.");

            var uf = UserDatabaseFactory.Create(FederationContext);
            output.WriteLine("User database module: {0}", uf.GetType().FullName);
        }

        public override IEnumerable<Graywulf.Check.CheckRoutineBase> GetCheckRoutines()
        {
            var uf = UserDatabaseFactory.Create(FederationContext);
            return uf.GetCheckRoutines();
        }
    }
}
