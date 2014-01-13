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

        [Option(Name = "Cluster", Description = "Export hardware info", Required = false)]
        public bool Cluster { get; set; }

        [Option(Name = "Federation", Description = "Export federation info", Required = false)]
        public bool Federation { get; set; }

        [Option(Name = "Layout", Description = "Export layout info", Required = false)]
        public bool Layout { get; set; }

        [Option(Name = "Jobs", Description = "Export job info", Required = false)]
        public bool Jobs { get; set; }

        [Option(Name = "NoUserJobs", Description = "Exclude user jobs", Required = false)]
        public bool NoUserJobs { get; set; }

        [Option(Name = "Security", Description = "Export security info", Required = false)]
        public bool Security { get; set; }

        public override void Run()
        {
            base.Run();

            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var f = new EntityFactory(context);
                var entity = f.LoadEntity(EntityName);

                // TODO: move masking logic to entity factory!

                HashSet<EntityType> mask;
                
                // If no flags are specified, all entities are exported by default
                if (!Cluster && !Federation && !Layout && !Jobs && !Security)
                {
                    mask = null;
                }
                else
                {
                    mask = new HashSet<EntityType>();

                    // Generate the entity masks
                    // Entities 'cluster' and 'domain' are always exported.

                    if (!Cluster)
                    {
                        mask.Add(EntityType.DiskVolume);
                        mask.Add(EntityType.Machine);
                        mask.Add(EntityType.MachineRole);
                        mask.Add(EntityType.ServerInstance);
                        mask.Add(EntityType.ServerVersion);
                    }

                    if (!Federation)
                    {
                        mask.Add(EntityType.Federation);
                        mask.Add(EntityType.DatabaseDefinition);
                        mask.Add(EntityType.DatabaseVersion);
                        mask.Add(EntityType.DistributedPartitionedView);
                        mask.Add(EntityType.FileGroup);
                        mask.Add(EntityType.Partition);
                        mask.Add(EntityType.RemoteDatabase);
                        mask.Add(EntityType.Slice);
                    }

                    if (!Layout)
                    {
                        mask.Add(EntityType.DatabaseInstance);
                        mask.Add(EntityType.DatabaseInstanceFile);
                        mask.Add(EntityType.DatabaseInstanceFileGroup);
                        mask.Add(EntityType.UserDatabaseInstance);
                    }

                    if (!Jobs)
                    {
                        mask.Add(EntityType.JobDefinition);
                        mask.Add(EntityType.JobInstance);
                        mask.Add(EntityType.QueueDefinition);
                        mask.Add(EntityType.QueueInstance);
                    }

                    if (!Security)
                    {
                        mask.Add(EntityType.User);
                        mask.Add(EntityType.UserGroup);
                        mask.Add(EntityType.UserGroupMembership);
                    }
                }

                using (var outfile = new StreamWriter(Output))
                {
                    f.Serialize(entity, outfile, mask, NoUserJobs);
                }
            }
        }
    }
}
