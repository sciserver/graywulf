using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.AccessControl;

namespace Jhu.Graywulf.Web.Services
{
    public abstract class RestServiceBase : IDisposable
    {
        private RegistryContext registryContext;
        private FederationContext federationContext;

        protected RegistryContext RegistryContext
        {
            get
            {
                if (registryContext != null)
                {
                    return registryContext;
                }
                else if (RestOperationContext.Current != null)
                {
                    return RestOperationContext.Current.RegistryContext;
                }
                else
                {
                    throw new InvalidOperationException("Registry context is not available");
                }
            }
        }

        protected FederationContext FederationContext
        {
            get
            {
                if (federationContext != null)
                {
                    return federationContext;
                }
                else if (RestOperationContext.Current != null)
                {
                    return RestOperationContext.Current.FederationContext;
                }
                else
                {
                    throw new InvalidOperationException("Federation context is not available");
                }
            }
        }
        
        protected RestServiceBase()
        {
            this.registryContext = null;
            this.federationContext = null;
        }

        protected RestServiceBase(FederationContext federationContext)
        {
            this.registryContext = federationContext.RegistryContext;
            this.federationContext = federationContext;
        }

        public virtual void Dispose()
        {
        }

        #region Request lifecycle hooks

        /// <summary>
        /// Called by the infrastructure before each service operation
        /// </summary>
        internal protected virtual void OnBeforeInvoke(RestOperationContext context)
        {
        }

        /// <summary>
        /// Called by the infrastucture after each service operation
        /// </summary>
        internal protected virtual void OnAfterInvoke(RestOperationContext context)
        {
        }

        internal protected void OnError(RestOperationContext context, Exception ex)
        {
        }

        #endregion
        #region User managemenet functions

        /// <summary>
        /// Called when a user signs in
        /// </summary>
        internal void OnUserArrived(GraywulfPrincipal principal)
        {
            /*
            using (var context = CreateRegistryContext())
            {
                // Check if user database (MYDB) exists, and create it if necessary
                // TODO: add this back after factoring API out from web project
                //var udii = new UserDatabaseInstanceInstaller(context);
                //udii.EnsureUserDatabaseInstanceExists(principal.Identity.User, context.Federation.MyDBDatabaseVersion);
            }
            */
        }

        /// <summary>
        /// Called when a user sings out
        /// </summary>
        /// <remarks>
        /// We cannot detect this since there's no session
        /// </remarks>
        internal void OnUserSignedOut(GraywulfPrincipal principaly)
        {
        }

        #endregion
        #region Predefined operations

        /// <summary>
        /// Dummy function to handle requests to OPTIONS
        /// </summary>
        public void HandleHttpOptionsRequest()
        {
        }

        public Stream GenerateProxy(string lang)
        {
            // TODO: add support for other languages
            var context = WebOperationContext.Current;
            var response = context.OutgoingResponse;


            var service = OperationContext.Current.Host.Description;
            var endpoint = (WebHttpEndpoint)service.Endpoints[0];
            var contractType = endpoint.Contract.ContractType;
            var serviceUrl = endpoint.ListenUri.AbsoluteUri;

            var cg = new JavascriptProxyGenerator(contractType, serviceUrl);
            response.Headers[HttpResponseHeader.ContentType] = cg.MimeType;
            response.Headers[HttpResponseHeader.CacheControl] = "public";
            response.Headers["Content-Disposition"] = "attachment;filename=\"" + cg.Filename + "\"";

            var ms = new MemoryStream();
            cg.Execute(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        #endregion
    }
}