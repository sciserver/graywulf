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
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// Implements a module that can authenticate REST web service clients
    /// based on the information in the request header.
    /// </summary>
    class RestAuthenticationModule : Security.AuthenticationModuleBase, IDispatchMessageInspector, IParameterInspector
    {
        // TODO: use this to cache authenticated identities
        private Cache<string, GraywulfPrincipal> principalCache;
        
        public RestAuthenticationModule()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.principalCache = new Cache<string, GraywulfPrincipal>()
            {
                AutoExtendLifetime = true,
                CollectionInterval = new TimeSpan(0, 1, 0),     // one minute
                DefaultLifetime = new TimeSpan(0, 20, 0),       // twenty minutes
            };
        }

        public void Init(Domain domain)
        {
            // Create authenticators
            var af = AuthenticatorFactory.Create(domain);
            RegisterAuthenticators(af.GetRestRequestAuthenticators());
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

            CallAuthenticators(httpContext);
            
            // Based on the results of the authentication, create a graywulf
            // uses
            var principal = DispatchIdentityType(httpContext.User);

            if (principal != null)
            {
                // TODO
                // Since there is no session, there's no way to raise the other event
                // that signals when a user is leaving

                httpContext.User = principal;
                System.Threading.Thread.CurrentPrincipal = principal;
            }

            IdentifyUser();
            
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

        private void IdentifyUser()
        {
            var httpContext = HttpContext.Current;
            var httpApplication = (ApplicationBase)HttpContext.Current.ApplicationInstance;

            // User is not recognized
            if (!(httpContext.User is GraywulfPrincipal))
            {
                throw new Jhu.Graywulf.Registry.SecurityException(ExceptionMessages.AccessDenied);
            }
            
            // TODO
            // REST services do not use a session but we don't want to load the user
            // every single time from the session so do some caching here.
            // The problem is, however, that we don't want to keep the user in the cache
            // for ever, so some cache expiration should be done

            var principal = (GraywulfPrincipal)httpContext.User;

            GraywulfPrincipal cachedPrincipal;
            if (!principalCache.TryGetValue(principal.Identity.Name, out cachedPrincipal))
            {
                // User not found in cache, need to load from database
                principal.Identity.LoadUser();

                principalCache.TryAdd(principal.Identity.Name, principal);

                cachedPrincipal = principal;

                // Also, we have to be able to detect users who just arrived so the appropriate
                // event can be raised. Now simply rise the event every time
                httpApplication.OnUserSignedIn(principal.Identity);
            }

            httpContext.User = cachedPrincipal;
        }
    }
}
