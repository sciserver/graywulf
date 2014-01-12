using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Registry.CmdLineUtil
{
    [Verb(Name = "Merge", Description = "Merges an XML file into the registry")]
    class Merge : Verb
    {
        [Parameter(Name = "Input", Description = "Name of input file", Required = true)]
        public string Input { get; set; }

        public override void Run()
        {
            base.Run();

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var f = new EntityFactory(context);

                using (var infile = new StreamReader(Input))
                {
                    f.Deserialize(infile);
                }
            }
        }
    }
}
