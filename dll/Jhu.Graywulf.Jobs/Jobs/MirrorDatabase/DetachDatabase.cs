using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.MirrorDatabase
{
    public class DetachDatabase : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<JobContext> JobContext { get; set; }

        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<Guid> DatabaseInstanceGuid { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            Guid databaseinstanceguid = DatabaseInstanceGuid.Get(activityContext);

            using (RegistryContext context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                DatabaseInstance di = new DatabaseInstance(context);
                di.Guid = databaseinstanceguid;
                di.Load();
                di.Detach();
            }

            EntityGuid.Set(activityContext, databaseinstanceguid);
        }
    }
}
