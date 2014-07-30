using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Web;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// Implements a module that can authenticate REST web service clients
    /// based on the information in the request header.
    /// </summary>
    class RestAuthenticationModule : Security.AuthenticationModuleBase, IDispatchMessageInspector, IParameterInspector, IDisposable
    {
        // TODO: use this to cache authenticated identities
        private Cache<string, GraywulfPrincipal> principalCache;

        #region Constructors and initializers

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

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion

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
            Authenticate(new AuthenticationRequest(WebOperationContext.Current.IncomingRequest));

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

        protected override void OnAuthenticated(GraywulfPrincipal principal)
        {
            // Based on the results of the authentication, create a graywulf user
            principal = DispatchIdentityType(principal);
            System.Threading.Thread.CurrentPrincipal = principal;

            IdentifyUser();
        }

        protected override void OnAuthenticationFailed()
        {
            System.Threading.Thread.CurrentPrincipal = null;
        }

        private void IdentifyUser()
        {
            // REST services do not use a session but we don't want to load the user
            // every single time from the session so do some caching here.
            // The problem is, however, that we don't want to keep the user in the cache
            // for ever, so some cache expiration should be done

            var principal = (GraywulfPrincipal)System.Threading.Thread.CurrentPrincipal;

            GraywulfPrincipal cachedPrincipal;
            if (!principalCache.TryGetValue(principal.Identity.Name, out cachedPrincipal))
            {
                // User not found in cache, need to load from database
                principal.Identity.LoadUser();

                principalCache.TryAdd(principal.Identity.Name, principal);

                cachedPrincipal = principal;

                // Also, we have to be able to detect users who just arrived so the appropriate
                // event can be raised. Now simply rise the event every time

                // TODO: find a new place for this event
                //httpApplication.OnUserSignedIn(principal.Identity);
            }

            System.Threading.Thread.CurrentPrincipal = cachedPrincipal;
        }
    }
}
