using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.Activities.Tracking;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.Test
{
    public class TestAsyncTrackingRecord : GraywulfAsyncCodeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<string> Message { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            string message = activityContext.GetValue(Message);

            var r = new CustomTrackingRecord("test");
            r.Data.Add("message", message);
            activityContext.Track(r);

            return delegate (AsyncJobContext asyncContext)
            {
                var r2 = new CustomTrackingRecord("test");
                r2.Data.Add("message", message);
                asyncContext.Track(r);
            };
        }
    }
}
