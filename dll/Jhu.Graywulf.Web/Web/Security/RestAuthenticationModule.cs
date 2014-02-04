using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.Web;

namespace Jhu.Graywulf.Web.Security
{
    class RestAuthenticationModule : Security.AuthenticationModuleBase, IDispatchMessageInspector, IParameterInspector
    {
        public RestAuthenticationModule()
        {
            base.Init();
        }

        public object AfterReceiveRequest(ref Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            CallRequestAuthenticators(HttpContext.Current);
            DispatchIdentityType(HttpContext.Current);

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            return null;
        }

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
        }
    }
}
