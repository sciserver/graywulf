using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel;

namespace Jhu.Graywulf.ServiceModel
{
    public class ServiceSynchronizationContext : SynchronizationContext
    {
        private SynchronizationContext outerContext;
        private OperationContext operationContext;

        #region Constructors and initializers

        public ServiceSynchronizationContext()
        {
            InitializeMembers();

            this.outerContext = SynchronizationContext.Current;
            this.operationContext = OperationContext.Current;

            SynchronizationContext.SetSynchronizationContext(this);
        }

        public ServiceSynchronizationContext(ServiceSynchronizationContext old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.outerContext = null;
            this.operationContext = null;
        }

        private void CopyMembers(ServiceSynchronizationContext old)
        {
            this.outerContext = old.outerContext;
            this.operationContext = old.operationContext;
        }

        public override SynchronizationContext CreateCopy()
        {
            return new ServiceSynchronizationContext(this);
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

            context.Post(
                s =>
                    {
                        SynchronizationContext.SetSynchronizationContext(this);
                        OperationContext.Current = operationContext;

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
