using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.CommandLineParser.Test
{
    [Verb(Name="Run", Description="Executes a query")]
    class RunParameters
    {
        [Parameter(Name="Query", Description="Query to execute")]
        public string Query { get; set; }

        [Parameter(Name = "Quota")]
        public int Quota { get; set; }

        [Option(Name = "Batch", Description="This is a description")]
        public bool Batch { get; set; }

        [Parameter(Name = "Test")]
        public StringComparison Test { get; set; }
    }
}
