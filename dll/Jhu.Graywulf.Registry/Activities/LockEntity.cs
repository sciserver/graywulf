using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Activities
{
    public class LockEntity : JobCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<Guid> LockOwner { get; set; }

        protected override void OnExecute(CodeActivityContext activityContext)
        {
            var entityguid = EntityGuid.Get(activityContext);
            var lockOwner = LockOwner.Get(activityContext);

            using (var context = ContextManager.Instance.CreateReadWriteContext())
            {
                context.LockOwner = lockOwner;

                var ef = new EntityFactory(context);
                var e = ef.LoadEntity(entityguid);
                e.ObtainLock();
            }
        }
    }
}
