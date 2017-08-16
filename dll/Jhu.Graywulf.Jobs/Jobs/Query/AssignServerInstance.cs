using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Jobs.Query
{
    public class AssignServerInstance : JobCodeActivity, IJobActivity
    {
        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<QueryObject> QueryObject { get; set; }

        protected override void OnExecute(CodeActivityContext activityContext)
        {
            var queryObject = QueryObject.Get(activityContext);

            switch (queryObject.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    queryObject.InitializeQueryObject(null, null, true);
                    break;
                case ExecutionMode.Graywulf:
                    using (var context = ContextManager.Instance.CreateReadOnlyContext())
                    {
                        var scheduler = activityContext.GetExtension<IScheduler>();

                        queryObject.InitializeQueryObject(context, scheduler, false);
                        queryObject.AssignServerInstance();
                        EntityGuid.Set(activityContext, queryObject.AssignedServerInstance.Guid);
                    }
                    break;
            }
        }
    }
}
