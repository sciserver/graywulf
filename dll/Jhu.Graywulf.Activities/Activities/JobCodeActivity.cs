using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Activities
{
    public abstract class JobCodeActivity : CodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<JobInfo> JobInfo { get; set; }

        protected sealed override void Execute(CodeActivityContext activityContext)
        {
            new JobContext(this, activityContext).Push();
            new LoggingContext(LoggingContext.Current, true).Push();
            JobContext.Current.UpdateLoggingContext(LoggingContext.Current);
            
            OnExecute(activityContext);

            LoggingContext.Current.Pop();
            JobContext.Current.Pop();
        }

        protected abstract void OnExecute(CodeActivityContext context);
    }

    public abstract class JobCodeActivity<T> : CodeActivity<T>, IJobActivity
    {
        [RequiredArgument]
        public InArgument<JobInfo> JobInfo { get; set; }

        protected sealed override T Execute(CodeActivityContext context)
        {
            // Set up job context here

            var res = OnExecute(context);

            return res;
        }

        protected abstract T OnExecute(CodeActivityContext context);
    }
}
