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
        #region Constructors and initializers

        public RestAuthenticationModule()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        #endregion
        #region WCF service life-cycle methods

        public void Init(Domain domain)
        {
            // (0.) Called by the framework when the authenticator module
            // is created an attached to the WCF service endpoints

            // Create authenticators
            var af = AuthenticatorFactory.Create(domain);
            RegisterAuthenticators(af.GetRestRequestAuthenticators());
        }

        #endregion
        #region WCF invocation life-cycle methods

        /// <summary>
        /// Authenticates a request based on the headers
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
        public object AfterReceiveRequest(ref Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            // (1.) Called before the WCF operation is invoked

            Authenticate(new AuthenticationRequest(WebOperationContext.Current.IncomingRequest));

            // The return value is passes to BeforeSendReply
            return null;
        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            // (2.)

            // This can be used to authenticate a request based on function parameters
            // These are parsed parameters, not necessarily REST query strings.
            return null;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
            // (3.)

            // Required by the interface, not used.
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            // (4.)

            // Required by the interface, not used
            // This call could be used to pass cookies to the client
        }

        protected override void OnAuthenticated(GraywulfPrincipal principal)
        {
            // (5.)  Called after successfull authentication

            System.Threading.Thread.CurrentPrincipal = principal;
        }

        protected override void OnAuthenticationFailed()
        {
            // (6.)

            // This only means that the custom authenticators could not
            // identify the user, but it still might have been identified by
            // the web server (from Forms ticket, windows authentication, etc.)
            // In this case, the principal provided by the framework needs to
            // be converted to a graywulf principal

            var principal = DispatchPrincipal(System.Threading.Thread.CurrentPrincipal);
            System.Threading.Thread.CurrentPrincipal = principal;
        }

        #endregion
    }
}
