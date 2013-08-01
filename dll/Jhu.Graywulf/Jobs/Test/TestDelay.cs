using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Jobs.Test
{
    public class TestDelay : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        [RequiredArgument]
        public InArgument<int> DelayPeriod { get; set; }

        [RequiredArgument]
        public InArgument<bool> Cancelable { get; set; }

        protected override IAsyncResult BeginExecute(AsyncCodeActivityContext activityContext, AsyncCallback callback, object state)
        {
            var period = DelayPeriod.Get(activityContext);
            var cancelable = Cancelable.Get(activityContext);
            
            Guid workflowInstanceGuid = activityContext.WorkflowInstanceId;
            string activityInstanceId = activityContext.ActivityInstanceId;
            return EnqueueAsync(_ => OnAsyncExecute(workflowInstanceGuid, activityInstanceId, period, cancelable), callback, state);
        }

        private void OnAsyncExecute(Guid workflowInstanceGuid, string activityInstanceId, int period, bool cancelable)
        {
            if (cancelable)
            {
                var delay = new CancelableDelay(period);

                RegisterCancelable(workflowInstanceGuid, activityInstanceId, delay);
                delay.Execute();
                UnregisterCancelable(workflowInstanceGuid, activityInstanceId, delay);
            }
            else
            {
                // This would idle
                //Thread.Sleep(period);

                // This doesn't idle
                var start = DateTime.Now;
                while ((DateTime.Now - start).TotalMilliseconds < period)
                {
                }
            }
        }

    }
}
