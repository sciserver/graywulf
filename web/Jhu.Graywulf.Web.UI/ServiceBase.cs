using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Web;
using System.ServiceModel;
using Jhu.Graywulf.Security;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.UI
{
    public abstract class ServiceBase
    {
        private Context registryContext;
        private FederationContext federationContext;

        protected HttpContext HttpContext
        {
            get { return HttpContext.Current; }
        }

        /// <summary>
        /// Gets the authenticated Graywulf user
        /// </summary>
        protected User RegistryUser
        {
            get
            {
                if (HttpContext.Current.User.Identity is GraywulfIdentity)
                {
                    var identity = (GraywulfIdentity)HttpContext.User.Identity;
                    identity.UserProperty.Context = RegistryContext;
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
                    var application = (ApplicationBase)HttpContext.ApplicationInstance;
                    registryContext = application.CreateRegistryContext();
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

        protected void HandleException(Exception ex)
        {
            WebOperationContext.Current.OutgoingResponse.SuppressEntityBody = true;
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";    // TODO: use constant

            HttpContext.Response.Write(ex.Message);
        }
    }
}