using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Jhu.Graywulf.Entities.Build
{
    class Program
    {
        static void Main(string[] args)
        {
            var script = new StringBuilder(File.ReadAllText(args[0]));
            var writer = new StringWriter();

            using (var infile = new FileStream(args[1], FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var buffer = new byte[0x10000];
                int res;

                while ((res = infile.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < res; i++)
                    {
                        writer.Write(BitConverter.ToString(buffer, i, 1));
                    }
                }
            }

            script.Replace("[$bin]", writer.ToString());

            File.WriteAllText(args[2], script.ToString());
        }
    }
}
