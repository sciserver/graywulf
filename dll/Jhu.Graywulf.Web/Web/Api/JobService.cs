using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web.Routing;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Web.Api
{
    [ServiceContract]
    public interface IJobService
    {
        [OperationContract]
        [WebGet(UriTemplate="{queue}")]
        Job[] GetJob(string queue);
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [RestServiceBehavior]
    public class JobService : ServiceBase, IJobService
    {
        public static void RegisterRoute()
        {
            RouteTable.Routes.Add(new ServiceRoute("Api/Jobs/", new WebServiceHostFactory(), typeof(Api.JobService)));
        }

        public Job[] GetJob(string queue)
        {
            throw new NotImplementedException();
        }
    }
}