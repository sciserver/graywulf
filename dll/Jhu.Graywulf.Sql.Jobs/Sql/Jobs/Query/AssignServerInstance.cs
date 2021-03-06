﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Sql.Jobs.Query
{
    public class AssignServerInstance : JobCodeActivity, IJobActivity
    {
        public OutArgument<Guid> EntityGuid { get; set; }

        [RequiredArgument]
        public InArgument<QueryObject> QueryObject { get; set; }

        protected override void OnExecute(CodeActivityContext activityContext)
        {
            var queryObject = QueryObject.Get(activityContext);

            switch (queryObject.Parameters.ExecutionMode)
            {
                case ExecutionMode.SingleServer:
                    queryObject.InitializeQueryObject(null, null, null, true);
                    break;
                case ExecutionMode.Graywulf:
                    using (var registryContext = ContextManager.Instance.CreateReadOnlyContext())
                    {
                        var scheduler = activityContext.GetExtension<IScheduler>();

                        queryObject.InitializeQueryObject(null, registryContext, scheduler, false);
                        queryObject.AssignServerInstance();
                        EntityGuid.Set(activityContext, queryObject.AssignedServerInstance.Guid);
                    }
                    break;
            }
        }
    }
}
