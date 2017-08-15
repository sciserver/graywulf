using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Registry.Check
{
    public class EntityCheck : CheckRoutineBase
    {
        public override CheckCategory Category
        {
            get
            {
                return CheckCategory.Registry;
            }
        }
        public RegistryContext Context { get; set; }
        public string Name { get; set; }

        public EntityCheck(RegistryContext context, string name)
        {
            this.Context = context;
            this.Name = name;
        }

        protected override IEnumerable<CheckRoutineStatus> OnExecute()
        {
            yield return ReportInfo("Testing registry entry: {0}", Name);

            var ef = new EntityFactory(Context);
            var e = ef.LoadEntity(Name);

            yield return ReportSuccess("Entry retrieved: {0}", e.Guid);
        }
    }
}
