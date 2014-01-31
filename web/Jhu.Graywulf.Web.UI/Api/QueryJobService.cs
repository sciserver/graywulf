using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Web.UI.Api
{
    [ServiceContract]
    public interface IQueryJobService
    {
        [OperationContract]
        [WebGet(UriTemplate="{queue}")]
        JobInstance[] GetJob(string queue);
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class QueryJobService : ServiceBase, IQueryJobService
    {
        public JobInstance[] GetJob(string queue)
        {
            try
            {
                throw new Exception("message");
            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }
        }
    }
}