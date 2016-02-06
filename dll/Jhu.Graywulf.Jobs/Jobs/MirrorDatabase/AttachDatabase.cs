using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.MirrorDatabase
{
    public class AttachDatabase : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<Guid> DatabaseInstanceGuid { get; set; }

        [RequiredArgument]
        public InArgument<bool> AttachReadOnly { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            Guid databaseinstanceguid = DatabaseInstanceGuid.Get(activityContext);
            bool attachReadOnly = AttachReadOnly.Get(activityContext);

            EntityGuid.Set(activityContext, databaseinstanceguid);

            using (Context context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                DatabaseInstance di = new DatabaseInstance(context);
                di.Guid = databaseinstanceguid;
                di.Load();
                di.Attach(attachReadOnly);
            }
        }
    }
}
