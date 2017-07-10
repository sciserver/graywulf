using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Activities
{
    public class LockEntity : CodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<JobInfo> JobInfo { get; set; }

        [RequiredArgument]
        public InArgument<Guid> EntityGuid { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            var entityguid = EntityGuid.Get(activityContext);

            using (var context = ContextManager.Instance.CreateContext(this, activityContext, ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var ef = new EntityFactory(context);

                var e = ef.LoadEntity(entityguid);
                e.ObtainLock();
            }
        }
    }
}
