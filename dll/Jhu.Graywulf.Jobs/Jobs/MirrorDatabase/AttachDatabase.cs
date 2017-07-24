using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.MirrorDatabase
{
    public class AttachDatabase : JobCodeActivity, IJobActivity
    {
        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<Guid> DatabaseInstanceGuid { get; set; }

        [RequiredArgument]
        public InArgument<bool> AttachReadOnly { get; set; }

        protected override void OnExecute(CodeActivityContext activityContext)
        {
            Guid databaseinstanceguid = DatabaseInstanceGuid.Get(activityContext);
            bool attachReadOnly = AttachReadOnly.Get(activityContext);

            EntityGuid.Set(activityContext, databaseinstanceguid);

            using (RegistryContext context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                DatabaseInstance di = new DatabaseInstance(context);
                di.Guid = databaseinstanceguid;
                di.Load();
                di.Attach(attachReadOnly);
            }
        }
    }
}
