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

namespace Jhu.Graywulf.Web.Api
{
    public abstract class ServiceBase : IDisposable
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

        protected ServiceBase()
        {
        }

        public virtual void Dispose()
        {
            if (registryContext != null)
            {
                if (registryContext.DatabaseTransaction != null)
                {
                    registryContext.CommitTransaction();
                }

                registryContext.Dispose();
                registryContext = null;
            }
        }

        /// <summary>
        /// Gets an initialized registry context.
        /// </summary>
        public Context CreateRegistryContext()
        {
            var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, Registry.TransactionMode.ManualCommit);
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
        }

        internal void OnError(string operationName, Exception ex)
        {
            LogError(operationName, ex);

            if (registryContext != null)
            {
                registryContext.RollbackTransaction();
                registryContext.Dispose();
                registryContext = null;
            }
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
    }
}