using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Registry.Jobs.MirrorDatabase
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

            using (RegistryContext context = ContextManager.Instance.CreateReadWriteContext())
            {
                var ef = new EntityFactory(context);
                var di = ef.LoadEntity<DatabaseInstance>(databaseinstanceguid);
                di.Attach(attachReadOnly);
            }
        }
    }
}
