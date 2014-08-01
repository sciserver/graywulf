using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.ServiceModel.Security;
using System.ServiceModel.Channels;
using System.Web.Routing;
using System.Security.Permissions;
using System.IO;

namespace Jhu.Graywulf.Web.Api.V1
{
    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/exception")]
        void ThrowException();
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [RestServiceBehavior]
    public class TestService : ServiceBase, ITestService
    {
        public void ThrowException()
        {
            throw new NotImplementedException();
        }
    }

}
