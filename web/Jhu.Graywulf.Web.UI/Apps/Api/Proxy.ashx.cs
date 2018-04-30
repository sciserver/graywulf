using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using Jhu.Graywulf.Web.Services;
using Jhu.Graywulf.Web.Services.CodeGen;

namespace Jhu.Graywulf.Web.UI.Apps.Api
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class Proxy : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            // Reflect services

            var application = (UIApplicationBase)HttpContext.Current.ApplicationInstance;
            var reflector = new RestServiceReflector();

            foreach (var service in application.Services)
            {
                var snattr = (ServiceNameAttribute)service.GetCustomAttributes(typeof(ServiceNameAttribute), false)[0];
                var url = String.Format("{0}/{1}.svc", snattr.Version, snattr.Name);

                reflector.ReflectServiceContract(service, url);
            }

            // Figure out which code generator to use
            // TODO: this could be made more generic with a factory class

            var contentType = context.Request.Headers["Content-Type"];
            var target = context.Request.QueryString["target"];
            RestProxyGeneratorBase cg = null;

            if (target == "js" ||
                Jhu.Graywulf.Util.MediaTypeComparer.Compare(contentType, "application/javascript"))
            {
                cg = new JavascriptProxyGenerator(reflector.Api);
            }
            else if (target == "json" ||
                Jhu.Graywulf.Util.MediaTypeComparer.Compare(contentType, "application/json"))
            {
                cg = new SwaggerJsonGenerator(reflector.Api);
            }
            else
            {
                throw new NotImplementedException();
            }

            context.Response.ContentType = cg.MimeType;
            cg.Execute(context.Response.Output);
        }
    }
}