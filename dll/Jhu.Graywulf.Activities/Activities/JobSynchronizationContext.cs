using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Activities
{
    public class JobSynchronizationContext : SynchronizationContext
    {
        private SynchronizationContext outerContext;

        #region Constructors and initializers

        public JobSynchronizationContext()
        {
            InitializeMembers();
        }

        public JobSynchronizationContext(JobSynchronizationContext old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.outerContext = null;
        }

        private void CopyMembers(JobSynchronizationContext old)
        {
            this.outerContext = old.outerContext;
        }

        public override SynchronizationContext CreateCopy()
        {
            return new JobSynchronizationContext(this);
        }

        #endregion

        /// <summary>
        /// Queue an async delegate using the default context and set the
        /// WCF opeartion context on the new thread.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="state"></param>
        public override void Post(SendOrPostCallback d, object state)
        {
            var context = outerContext ?? new SynchronizationContext();
            var jobContext = JobContext.Current;
            var loggingContext = LoggingContext.Current;

            context.Post(
                s =>
                {
                    SynchronizationContext.SetSynchronizationContext(this);
                    JobContext.Current = jobContext;
                    LoggingContext.Current = loggingContext;

                    try
                    {
                        d(s);
                    }
                    catch (Exception)
                    {
                        // If we didn't have this, async void would be bad news bears.
                        // Since async void is "fire and forget," they happen separate
                        // from the main call stack.  We're logging this separately so
                        // that they don't affect the main call (and it just makes sense).

                        // TODO: log here
                    }
                },
                state
            );
        }
    }
}
