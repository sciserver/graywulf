using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Registry.CmdLineUtil
{
    [Verb(Name = "Import", Description = "Imports an XML file into the registry")]
    class Import : Verb
    {
        [Parameter(Name = "Input", Description = "Name of input file", Required = true)]
        public string Input { get; set; }

        [Option(Name = "Duplicates", Description = "Duplicate merge method")]
        public DuplicateMergeMethod DuplicateMergeMethod { get; set; }

        public override void Run()
        {
            base.Run();

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.ManualCommit))
            {
                try
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
                catch (Exception ex)
                {
                    Console.WriteLine("Importing xml file failed.");
                    Console.WriteLine(ex.Message);

                    context.RollbackTransaction();
                }
            }
        }
    }
}
