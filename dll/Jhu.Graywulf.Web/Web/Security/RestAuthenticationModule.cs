using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Web;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// Implements a module that can authenticate REST web service clients
    /// based on the information in the request header.
    /// </summary>
    class RestAuthenticationModule : Security.AuthenticationModuleBase, IDispatchMessageInspector, IParameterInspector
    {
        private string authenticatorFactory;

        // TODO: use this to cache authenticated identities
        private ConcurrentDictionary<string, GraywulfPrincipal> principalCache;
        
        public RestAuthenticationModule()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.principalCache = new ConcurrentDictionary<string, GraywulfPrincipal>(StringComparer.InvariantCultureIgnoreCase);
        }

        public void Init(Domain domain)
        {
            // Create authenticators
            var af = AuthenticatorFactory.Create(domain);
            RegisterRequestAuthenticators(af.CreateRestRequestAuthenticators());
        }

        /// <summary>
        /// Authenticates a request based on the headers
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
        public object AfterReceiveRequest(ref Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            var httpContext = HttpContext.Current;
            var httpApplication = (ApplicationBase)HttpContext.Current.ApplicationInstance;

            CallRequestAuthenticators(httpContext);
            
            // Based on the results of the authentication, create a graywulf
            // used
            var user = DispatchIdentityType(httpContext.User);

            if (user != null)
            {
                // TODO
                // REST services do not use a session but we don't want to load the user
                // every single time from the session so do some caching here.
                // The problem is, however, that we don't want to keep the user in the cache
                // for ever, so some cache expiration should be done
                using (Registry.Context registryContext = httpApplication.CreateRegistryContext())
                {
                    user.Identity.LoadUser(registryContext.Domain);
                }

                // TODO
                // Also, we have to be able to detect users who just arrived so the appropriate
                // event can be raised. Now simply rise the event every time
                httpApplication.OnUserSignedIn(user.Identity);

                // TODO
                // Since there is no session, there's no way to raise the other event
                // that signals when a user is leaving
                
                System.Threading.Thread.CurrentPrincipal = user;
            }
            
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            // Required by the interface, not used
        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            // This can be used to authenticate a request based on function parameters
            // These are parsed parameters, not necessarily REST query strings.

            return null;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            // Required by the interface, not used.
        }
    }
}
