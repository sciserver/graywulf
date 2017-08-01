using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Jhu.Graywulf.ServiceModel
{
    /// <summary>
    /// Implements a custom invoker that performs simple role-based authorization
    /// before each operation call on a remote service, at server-side.
    /// </summary>
    class LimitedAccessOperationInvoker : IOperationInvoker
    {
        private string operationName;
        private string operationCategory;
        private IOperationInvoker originalInvoker;

        public LimitedAccessOperationInvoker(string operationName, string operationCategory, IOperationInvoker originalInvoker)
        {
            this.operationName = operationName;
            this.operationCategory = operationCategory;
            this.originalInvoker = originalInvoker;
        }

        public bool IsSynchronous
        {
            get { return this.originalInvoker.IsSynchronous; }
        }

        /// <summary>
        /// Allocates objects that will be used as parameters for the function call
        /// </summary>
        /// <returns></returns>
        public object[] AllocateInputs()
        {
            return originalInvoker.AllocateInputs();
        }


        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            EnsureRoleAccess();

            return this.originalInvoker.Invoke(instance, inputs, out outputs);
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return this.originalInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return this.originalInvoker.InvokeEnd(instance, out outputs, result);
        }

        /// <summary>
        /// Authorizes the user only if they belong to a specified user group.
        /// </summary>
        public void EnsureRoleAccess()
        {
            // Access automatically granted for non-remoting scenarios.
            // OperationContext.Current is null if the object is created locally.
            // Otherwise check if the user is authenticated and the identity is equal to the specified
            // user (group) name or member of the given group (role).

            var context = OperationContext.Current;

            if (context != null)
            {
                var access = context.Host.Extensions.Find<LimitedAccessServiceExtension>();

                if (access.UserList[operationCategory].Contains(Thread.CurrentPrincipal.Identity.Name))
                {
                    return;
                }

                foreach (var role in access.RoleList[operationCategory])
                {
                    if (Thread.CurrentPrincipal.IsInRole(role))
                    {
                        return;
                    }
                }
            }
        }
    }
}
