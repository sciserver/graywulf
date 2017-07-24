﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Jobs.Query
{
    public class CreateDestinationTablePrimaryKey : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<SqlQuery> Query { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            var workflowInstanceId = activityContext.WorkflowInstanceId;
            var activityInstanceId = activityContext.ActivityInstanceId;
            var query = Query.Get(activityContext);
            var queryPartition = query.Partitions[0];
            Table destinationTable;

            using (RegistryContext context = query.CreateContext())
            {
                queryPartition.InitializeQueryObject(context, null, true);
                queryPartition.PrepareCreateDestinationTablePrimaryKey(context, activityContext.GetExtension<IScheduler>(), out destinationTable);
            }

            return delegate ()
            {
                RegisterCancelable(workflowInstanceId, activityInstanceId, queryPartition);
                queryPartition.CreateDestinationTablePrimaryKey(destinationTable);
                UnregisterCancelable(workflowInstanceId, activityInstanceId, queryPartition);
            };
        }
    }
}
