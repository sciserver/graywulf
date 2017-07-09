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
        public InArgument<int> DelayPeriod { get; set; }

        [RequiredArgument]
        public InArgument<bool> Cancelable { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            var period = DelayPeriod.Get(activityContext);
            var cancelable = Cancelable.Get(activityContext);

            return delegate (AsyncJobContext asyncContext)
            {
                if (cancelable)
                {
                    var delay = new CancelableDelay(period);

                    asyncContext.RegisterCancelable(delay);
                    delay.Execute();
                    asyncContext.UnregisterCancelable(delay);
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
            };
        }
    }
}
