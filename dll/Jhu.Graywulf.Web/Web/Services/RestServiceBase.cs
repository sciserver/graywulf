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
        private RegistryContext registryContext;
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
                    identity.User.RegistryContext = RegistryContext;
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
        protected RegistryContext RegistryContext
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
        public RegistryContext CreateRegistryContext()
        {
            var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, Registry.TransactionMode.ManualCommit);

            if (Thread.CurrentPrincipal is GraywulfPrincipal)
            {
                var identity = (GraywulfIdentity)Thread.CurrentPrincipal.Identity;
                context.UserReference.Value = identity.User;
            }

            return context;
        }

        internal void OnBeforeInvoke()
        {
            new Logging.WebLoggingContext(Logging.LoggingContext.Current).Push();
            Logging.WebLoggingContext.Current.DefaultEventSource = Logging.EventSource.WebService;
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

            Logging.WebLoggingContext.Current.Pop();
        }

        internal void OnError(Exception ex)
        {
            if (registryContext != null)
            {
                registryContext.RollbackTransaction();
                registryContext.Dispose();
                registryContext = null;
            }
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
        #region Logging

        internal Logging.Event LogOperation()
        {
            var e = Logging.LoggingContext.Current.CreateEvent(
                Logging.EventSeverity.Operation,
                Logging.EventSource.WebService,
                null,
                null,
                null,
                null);

            UpdateEvent(e);
            Logging.LoggingContext.Current.WriteEvent(e);

            return e;
        }

        internal Logging.Event LogError(Exception ex)
        {
            var e = Logging.LoggingContext.Current.CreateEvent(
                Logging.EventSeverity.Error,
                Logging.EventSource.WebService,
                null,
                null,
                ex,
                null);

            UpdateEvent(e);
            Logging.LoggingContext.Current.WriteEvent(e);

            return e;
        }

        private void UpdateEvent(Logging.Event e)
        {
            string message = null;
            string operation = null;

            var context = System.ServiceModel.OperationContext.Current;

            if (context != null)
            {
                if (context.IncomingMessageProperties.ContainsKey("HttpOperationName"))
                {
                    operation = context.Host.Description.ServiceType.FullName + "." +
                        (string)context.IncomingMessageProperties["HttpOperationName"];
                }

                if (context.IncomingMessageProperties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    var req = (HttpRequestMessageProperty)context.IncomingMessageProperties["httpRequest"];

                    message = req.Method.ToUpper() + " " +
                        context.EndpointDispatcher.EndpointAddress.Uri.ToString();
                }
            }
            
            e.Message = message;
            e.Operation = operation;
        }

        #endregion
    }
}