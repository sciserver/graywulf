using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Registry.CmdLineUtil
{
    [Verb(Name = "Save", Description = "Saves a branch of the registry into an XML file")]
    class Save : Verb
    {
        [Parameter(Name = "Output", Description = "Name of output file", Required = true)]
        public string Output { get; set; }

        [Parameter(Name = "EntityName", Description = "Name of entity to serialize", Required = true)]
        public string EntityName { get; set; }

        public override void Run()
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var f = new EntityFactory(context);
                var entity = f.LoadEntity(EntityName);

                // *** TODO: create mask from input parameters
                var mask = EntityType.AllCluster | EntityType.AllDomain | EntityType.AllFederation | EntityType.AllLayou | EntityType.AllSecurity | EntityType.AllJobs;
                //mask = (EntityType)((int)mask & ~(0x0400));  // Leave out job instances

                using (var outfile = new StreamWriter(Output))
                {
                    f.Serialize(entity, outfile, mask);
                }
            }
        }
    }
}
