﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.Activities.Tracking;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Jobs.Test
{
    public class TestAsyncTrackingRecord : JobAsyncCodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<string> Message { get; set; }

        protected override AsyncActivityWorker OnBeginExecute(AsyncCodeActivityContext activityContext)
        {
            string message = activityContext.GetValue(Message);

            Logging.LoggingContext.Current.LogStatus(Logging.EventSource.Test, "Test event from async activitity initializer.");

            return delegate ()
            {
                Logging.LoggingContext.Current.LogStatus(Logging.EventSource.Test, "Test event from async activitity action.");
            };
        }
    }
}
