using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Jhu.Graywulf.Logging;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Activities
{
    public abstract class JobCodeActivity : CodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<JobInfo> JobInfo { get; set; }

        protected sealed override void Execute(CodeActivityContext activityContext)
        {
            new JobContext(this, activityContext).Push();

            using (var loggingContext = new LoggingContext(true))
            {
                JobContext.Current.UpdateLoggingContext(loggingContext);

                OnExecute(activityContext);

                loggingContext.FlushEvents(activityContext);
            }

            JobContext.Current.Pop();
        }

        protected abstract void OnExecute(CodeActivityContext activityContext);
    }

    public abstract class JobCodeActivity<T> : CodeActivity<T>, IJobActivity
    {
        [RequiredArgument]
        public InArgument<JobInfo> JobInfo { get; set; }

        protected sealed override T Execute(CodeActivityContext activityContext)
        {
            new JobContext(this, activityContext).Push();

            using (new LoggingContext(true))
            {
                JobContext.Current.UpdateLoggingContext(LoggingContext.Current);

                var res = OnExecute(activityContext);
                return res;
            }
        }

        protected abstract T OnExecute(CodeActivityContext context);
    }
}
