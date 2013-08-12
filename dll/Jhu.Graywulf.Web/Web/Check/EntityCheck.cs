using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Check
{
    public class EntityCheck : CheckRoutineBase
    {
        public string Name { get; set; }

        public EntityCheck(string name)
        {
            this.Name = name;
        }

        public override void Execute(PageBase page)
        {
            page.Response.Output.WriteLine(
                "Testing registry entry: {0}",
                Name);

            var ef = new EntityFactory(page.RegistryContext);
            var e = ef.LoadEntity(Name);

            page.Response.Output.WriteLine("Entry retrieved: {0}", e.Guid);
        }
    }
}
