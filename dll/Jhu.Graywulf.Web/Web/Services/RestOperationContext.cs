using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Web;
using System.Threading;
using Jhu.Graywulf.Web.UI;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.AccessControl;

namespace Jhu.Graywulf.Web.Services
{
    public class RestOperationContext : IExtension<OperationContext>, IDisposable
    {
        private OperationContext operationContext;
        private bool ownsContext;
        private RegistryContext registryContext;
        private FederationContext federationContext;
        private Dictionary<string, object> items;

        public IDictionary<string, object> Items
        {
            get { return items; }
        }

        public object this[string key]
        {
            get { return items[key]; }
            set { items[key] = value; }
        }

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
        public RegistryContext RegistryContext
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

        public FederationContext FederationContext
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

        public System.Security.Principal.IPrincipal Principal
        {
            get
            {
                if (System.Threading.Thread.CurrentPrincipal is Jhu.Graywulf.AccessControl.Principal)
                {
                    return (Jhu.Graywulf.AccessControl.Principal)System.Threading.Thread.CurrentPrincipal;
                }
                else
                {
                    return Jhu.Graywulf.AccessControl.Principal.Guest;
                }
            }
        }

        public IRestSessionState Session
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return new RestHttpSessionStateWrapper();
                }
                else
                {
                    // TODO: wcf session doesn't work, for some reason, but the IIS session
                    // should, so this never happens here

                    // If this happens for some reason, add to web.config:
                    /*
                    <configuration>
                    <system.serviceModel>
                    <serviceHostingEnvironment aspNetCompatibilityEnabled="true" />
                    </system.serviceModel>
                    </configuration>
                    */

                    var session = operationContext.InstanceContext.Extensions.Find<RestInstanceSessionState>();

                    if (session == null)
                    {
                        session = new RestInstanceSessionState();
                        OperationContext.Current.InstanceContext.Extensions.Add(session);
                    }

                    throw new NotImplementedException();
                }
            }
        }

        public static RestOperationContext Current
        {
            get
            {
                RestOperationContext context = OperationContext.Current.Extensions.Find<RestOperationContext>();
                return context;
            }
        }

        internal RestOperationContext()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.ownsContext = true;
            this.registryContext = null;
            this.federationContext = null; ;
            this.items = new Dictionary<string, object>();
        }

        public void Dispose()
        {
            if (ownsContext && federationContext != null)
            {
                federationContext.Dispose();
                federationContext = null;
            }

            if (ownsContext && registryContext != null)
            {
                registryContext.Dispose();
                registryContext = null;
            }
        }

        /// <summary>
        /// Gets an initialized registry context.
        /// </summary>
        public RegistryContext CreateRegistryContext()
        {
            var context = ContextManager.Instance.CreateReadWriteContext();

            if (Thread.CurrentPrincipal is GraywulfPrincipal)
            {
                var identity = (GraywulfIdentity)Thread.CurrentPrincipal.Identity;
                context.UserReference.Value = identity.User;
            }

            return context;
        }

        public void Attach(OperationContext owner)
        {
            this.operationContext = owner;
        }

        public void Detach(OperationContext owner)
        {
            this.operationContext = null;
        }
    }
}
