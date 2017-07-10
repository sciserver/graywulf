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

        public override void Execute(TextWriter output)
        {
            output.WriteLine(
                "Testing registry entry: {0}",
                Name);

            var ef = new EntityFactory(Context);
            var e = ef.LoadEntity(Name);

            output.WriteLine("Entry retrieved: {0}", e.Guid);
        }
    }
}
