using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install.CmdLineUtil
{
    [Verb(Name = "Export", Description = "Exports a branch of the registry into an XML file")]
    class Export : Verb
    {
        [Parameter(Name = "Output", Description = "Name of output file", Required = true)]
        public string Output { get; set; }

        [Parameter(Name = "RootEntity", Description = "Name of root entity to serialize", Required = true)]
        public string RootEntitity { get; set; }

        [Option(Name = "Object", Description = "Export single object", Required = false)]
        public bool Object { get; set; }

        [Option(Name = "Cluster", Description = "Export hardware info recursively", Required = false)]
        public bool Cluster { get; set; }

        [Option(Name = "Domain", Description = "Export domain info recursively", Required = false)]
        public bool Domain { get; set; }

        [Option(Name = "Federation", Description = "Export federation info recursively", Required = false)]
        public bool Federation { get; set; }

        [Option(Name = "Layout", Description = "Export layout info recursively", Required = false)]
        public bool Layout { get; set; }

        [Option(Name = "Jobs", Description = "Export job info recursively", Required = false)]
        public bool Jobs { get; set; }

        [Option(Name = "ExcludeUserCreated", Description = "Exclude user-created items", Required = false)]
        public bool ExcludeUserCreated { get; set; }

        protected override string OnGetConnectionString()
        {
            return Jhu.Graywulf.Registry.ContextManager.Configuration.ConnectionString;
        }

        public override void Run()
        {
            ContextManager.Instance.ConnectionString = GetConnectionString();

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var f = new EntityFactory(context);
                var entity = f.LoadEntity(RootEntitity);
                var s = new RegistrySerializer(entity)
                {
                    ExcludeUserCreated = ExcludeUserCreated
                };

                if (Object)
                {
                    using (var outfile = new StreamWriter(Output))
                    {
                        s.Recursive = false;
                        s.Serialize(outfile);
                    }
                }
                else
                {
                    if (Cluster || Domain || Federation || Layout || Jobs)
                    {
                        if (!Cluster) s.EntityGroupMask &= ~EntityGroup.Cluster;
                        if (!Domain) s.EntityGroupMask &= ~EntityGroup.Domain;
                        if (!Federation) s.EntityGroupMask &= ~EntityGroup.Federation;
                        if (!Layout) s.EntityGroupMask &= ~EntityGroup.Layout;
                        if (!Jobs) s.EntityGroupMask &= ~EntityGroup.Jobs;
                    }

                    using (var outfile = new StreamWriter(Output))
                    {
                        s.Recursive = true;
                        s.Serialize(outfile);
                    }
                }
            }
        }
    }
}
