using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.Check
{
    public class AssemblyCheck : CheckRoutineBase
    {
        public string Path { get; set; }

        public AssemblyCheck(string path)
        {
            this.Path = path;
        }

        public override void Execute(PageBase page)
        {
            page.Response.Output.WriteLine(
                "Testing assembly: {0}",
                Path);

            var a = Assembly.ReflectionOnlyLoadFrom(Path);

            page.Response.Output.WriteLine("Assembly found: {0}", a.FullName);
        }
    }
}
