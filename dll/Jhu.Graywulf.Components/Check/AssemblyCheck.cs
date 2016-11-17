using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Jhu.Graywulf.Check
{
    public class AssemblyCheck : CheckRoutineBase
    {
        public override CheckCategory Category
        {
            get
            {
                return CheckCategory.Assembly;
            }
        }

        public string Path { get; set; }

        public AssemblyCheck(string path)
        {
            this.Path = path;
        }

        public override void Execute(TextWriter output)
        {
            output.WriteLine(
                "Testing assembly: {0}",
                Path);

            var a = Assembly.ReflectionOnlyLoadFrom(Path);

            output.WriteLine("Assembly found: {0}", a.FullName);
        }
    }
}
