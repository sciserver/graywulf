using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.MirrorDatabase
{
    public class FindDatabaseInstanceFiles : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<JobContext> JobContext { get; set; }

        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<Guid> SourceDatabaseInstanceGuid { get; set; }
        [RequiredArgument]
        public OutArgument<Dictionary<Guid, List<Guid>>> SourceDatabaseInstanceFileGuids { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            Guid databaseinstanceguid = SourceDatabaseInstanceGuid.Get(activityContext);

            using (RegistryContext context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                DatabaseInstance di = new DatabaseInstance(context);
                di.Guid = databaseinstanceguid;
                di.Load();

                di.LoadFileGroups(false);

                // Files will be sorted by logical volume to allow parallel copies

                Dictionary<Guid, List<Guid>> guids = new Dictionary<Guid, List<Guid>>();

                // Append files
                foreach (DatabaseInstanceFileGroup fg in di.FileGroups.Values)
                {
                    fg.LoadFiles(false);

                    foreach (DatabaseInstanceFile f in fg.Files.Values)
                    {
                        if (!guids.ContainsKey(f.DiskVolume.Guid))
                        {
                            guids.Add(f.DiskVolume.Guid, new List<Guid>());
                        }

                        guids[f.DiskVolume.Guid].Add(f.Guid);
                    }
                }

                SourceDatabaseInstanceFileGuids.Set(activityContext, guids);
            }

            EntityGuid.Set(activityContext, databaseinstanceguid);
        }
    }
}
