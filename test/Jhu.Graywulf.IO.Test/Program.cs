using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var test = new IO.Tasks.ImportTableTest();
            test.ImportFromHttpTest();
        }
    }
}
