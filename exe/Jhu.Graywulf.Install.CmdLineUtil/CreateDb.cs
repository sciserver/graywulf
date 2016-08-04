using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.Install.CmdLineUtil
{
    public abstract class CreateDb : Verb
    {
        [Option(Name = "DbOnly", Description = "Database only, without schema")]
        public bool DbOnly { get; set; }

        [Option(Name = "SchemaOnly", Description = "Schema only, use existing database")]
        public bool SchemaOnly { get; set; }
    }
}
