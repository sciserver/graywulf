using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.ServiceModel.Web;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.Services
{
    public abstract class RestServiceBase : IDisposable
    {
        private Context registryContext;
        private FederationContext federationContext;

        protected string AppRelativePath
        {
            get
            {
                return VirtualPathUtility.ToAppRelative(
                    System.ServiceModel.OperationContext.Current.RequestContext.RequestMessage.Headers.To.PathAndQuery);
            }
        }

        /// <summary>
        /// Gets the authenticated Graywulf user
        /// </summary>
        protected User RegistryUser
        {
            get
            {
                if (Thread.CurrentPrincipal is GraywulfPrincipal)
                {
                    var identity = (GraywulfIdentity)Thread.CurrentPrincipal.Identity;
                    identity.User.Context = RegistryContext;
                    return identity.User;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets an initialized  registry context.
        /// </summary>
        protected Context RegistryContext
        {
            get
            {
                if (registryContext == null)
                {
                    registryContext = CreateRegistryContext();
                }

                return registryContext;
            }
        }

        protected FederationContext FederationContext
        {
            get
            {
                if (federationContext == null)
                {
                    federationContext = new FederationContext(RegistryContext, RegistryUser);
                }

                return federationContext;
            }
        }

        protected RestServiceBase()
        {
        }

        public virtual void Dispose()
        {
            if (registryContext != null)
            {
                registryContext.Dispose();
                registryContext = null;
            }
        }

        /// <summary>
        /// Dummy function to handle requests to OPTIONS
        /// </summary>
        public void HandleHttpOptionsRequest()
        {
        }

        /// <summary>
        /// Gets an initialized registry context.
        /// </summary>
        public Context CreateRegistryContext()
        {
            var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, Registry.TransactionMode.ManualCommit);

            if (Thread.CurrentPrincipal is GraywulfPrincipal)
            {
                var identity = (GraywulfIdentity)Thread.CurrentPrincipal.Identity;

                context.UserGuid = identity.UserReference.Guid;
                context.UserName = identity.UserReference.Name;
            }

            return context;
        }

        internal void OnBeforeInvoke()
        {
        }

        /// <summary>
        /// Called by the infrastucture after each service operation
        /// </summary>
        /// <param name="invoker"></param>
        internal void OnAfterInvoke()
        {
            if (registryContext != null && registryContext.DatabaseTransaction != null)
            {
                registryContext.CommitTransaction();
            }
        }

        internal Logging.Event OnError(string operationName, Exception ex)
        {
            var e = LogError(operationName, ex);

            if (registryContext != null)
            {
                registryContext.RollbackTransaction();
                registryContext.Dispose();
                registryContext = null;
            }

            return e;
        }

        private Logging.Event LogError(string operationName, Exception ex)
        {
            var error = Logging.Logger.Instance.LogException(
                AppRelativePath,
                Logging.EventSource.WebService,
                registryContext == null ? Guid.Empty : registryContext.UserGuid,
                registryContext == null ? Guid.Empty : registryContext.ContextGuid,
                ex);

            return error;
        }

        #region User managemenet functions

        /// <summary>
        /// Called when a user signs in
        /// </summary>
        internal void OnUserArrived(GraywulfPrincipal principal)
        {
            using (var context = CreateRegistryContext())
            {
                // Check if user database (MYDB) exists, and create it if necessary
                // TODO: add this back after factoring API out from web project
                //var udii = new UserDatabaseInstanceInstaller(context);
                //udii.EnsureUserDatabaseInstanceExists(principal.Identity.User, context.Federation.MyDBDatabaseVersion);
            }
        }

        /// <summary>
        /// Called when a user sings out
        /// </summary>
        internal void OnUserSignedOut(GraywulfPrincipal principaly)
        {
        }

        #endregion
    }
}