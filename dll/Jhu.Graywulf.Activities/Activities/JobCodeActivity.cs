using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;

namespace Jhu.Graywulf.Activities
{
    public abstract class JobCodeActivity : CodeActivity, IJobActivity
    {
        [RequiredArgument]
        public InArgument<JobInfo> JobInfo { get; set; }

        protected sealed override void Execute(CodeActivityContext activityContext)
        {
            var context = new JobContext(Logging.LoggingContext.Current, this, activityContext);

            context.Push();

            OnExecute(activityContext);

            context.Pop();
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
