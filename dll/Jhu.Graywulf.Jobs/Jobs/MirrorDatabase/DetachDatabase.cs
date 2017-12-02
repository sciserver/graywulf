using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Jobs.MirrorDatabase
{
    public class DetachDatabase : JobCodeActivity, IJobActivity
    {
        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<Guid> DatabaseInstanceGuid { get; set; }

        protected override void OnExecute(CodeActivityContext activityContext)
        {
            Guid databaseinstanceguid = DatabaseInstanceGuid.Get(activityContext);

            using (RegistryContext context = ContextManager.Instance.CreateReadWriteContext())
            {
                var ef = new EntityFactory(context);
                var di = ef.LoadEntity<DatabaseInstance>(databaseinstanceguid);
                di.Detach();
            }

            EntityGuid.Set(activityContext, databaseinstanceguid);
        }
    }
}
