using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install.CmdLineUtil
{
    [Verb(Name = "Import", Description = "Imports an XML file into the registry")]
    class Import : Verb
    {
        [Parameter(Name = "Input", Description = "Name of input file", Required = true)]
        public string Input { get; set; }

        [Parameter(Name = "Duplicates", Description = "Duplicate merge method")]
        public DuplicateMergeMethod DuplicateMergeMethod { get; set; }

        protected override string OnGetConnectionString()
        {
            return Jhu.Graywulf.Registry.ContextManager.Configuration.ConnectionString;
        }

        public override void Run()
        {
            ContextManager.Instance.ConnectionString = GetConnectionString();

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.ManualCommit))
            {
                var s = new RegistryDeserializer(context)
                {
                    DuplicateMergeMethod = DuplicateMergeMethod
                };

                using (var infile = new StreamReader(Input))
                {
                    s.Deserialize(infile);
                }

                context.CommitTransaction();
            }
        }
    }
}
