using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Registry
{
    public class LockEntity : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<Guid> EntityGuid { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            Guid entityguid = EntityGuid.Get(activityContext);

            using (Context context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                EntityFactory ef = new EntityFactory(context);

                Entity e = ef.LoadEntity(entityguid);
                e.ObtainLock();
            }
        }
    }
}
