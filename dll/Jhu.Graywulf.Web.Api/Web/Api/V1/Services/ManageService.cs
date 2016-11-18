using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using System.ComponentModel;
using Jhu.Graywulf.AccessControl;
using Jhu.Graywulf.Web.Security;
using Jhu.Graywulf.Web.Services;

namespace Jhu.Graywulf.Web.Api.V1
{
    [ServiceContract]
    [ServiceName(Name = "Manage", Version = "V1")]
    [Description("Management interface.")]
    public interface IManageService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "*", Method = "OPTIONS")]
        void HandleHttpOptionsRequest();

        [OperationContract]
        [WebGet(UriTemplate = "/schema/flush")]
        [Description("Flush the schema cache.")]
        void FlushSchema();
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [RestServiceBehavior]
    public class ManageService : RestServiceBase, IManageService
    {
        public void FlushSchema()
        {
            FederationContext.SchemaManager.Flush();
        }
    }
}
