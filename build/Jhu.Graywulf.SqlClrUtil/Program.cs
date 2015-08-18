using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace Jhu.Graywulf.SqlClrUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            var a = Assembly.LoadFrom(args[0]);

            var r = new SqlClrReflector(a);

            var name = a.GetName().Name;
            var dir = Path.GetDirectoryName(a.Location);

            using (var outfile = new StreamWriter(Path.Combine(dir, name + ".Create.sql")))
            {
                r.ScriptCreate(outfile);
            }

            using (var outfile = new StreamWriter(Path.Combine(dir, name + ".Drop.sql")))
            {
                r.ScriptDrop(outfile);
            }
        }
    }
}
