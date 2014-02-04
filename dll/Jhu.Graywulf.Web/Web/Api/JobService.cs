using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.ServiceModel.Security;
using System.Web.Routing;
using System.Security.Permissions;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Web.Api
{
    [ServiceContract]
    public interface IJobService
    {
        [OperationContract]
        [WebGet(UriTemplate = "hello")]
        string Hello();


        /// <summary>
        /// Returns all jobs of the user in the specified queue
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "{queue}")]
        Job[] GetJobs(string queue);

        /// <summary>
        /// Returns a job of the user in the specific queue, identified
        /// by a guid
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "{queue}?guid={guid}")]
        Job GetJob(string queue, string guid);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "{queue}/query")]
        IEnumerable<QueryJob> GetQueryJobs(string queue);

        [OperationContract]
        [WebGet(UriTemplate = "{queue}/query?guid={guid}")]
        QueryJob GetQueryJob(string queue, string guid);

        [OperationContract]
        [WebGet(UriTemplate = "{queue}/export")]
        ExportJob[] GetExportJobs(string queue);

        [OperationContract]
        [WebGet(UriTemplate = "{queue}/export?guid={guid}")]
        ExportJob GetExportJob(string queue, string guid);

        [OperationContract]
        [WebGet(UriTemplate = "{queue}/import")]
        ImportJob[] GetImportJobs(string queue);

        [OperationContract]
        [WebGet(UriTemplate = "{queue}/import?guid={guid}")]
        ImportJob GetImportJob(string queue, string guid);
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    //[PrincipalPermission(SecurityAction.Demand, Authenticated=true)]
    [RestServiceBehavior]
    public class JobService : ServiceBase, IJobService
    {
        public string Hello()
        {
            return string.Format("Hello {0}", System.Threading.Thread.CurrentPrincipal.Identity.Name);

        }

        public string Error()
        {
            throw new NotImplementedException();
        }

        public Job[] GetJobs(string queue)
        {
            throw new NotImplementedException();
        }

        public Job GetJob(string queue, string guid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<QueryJob> GetQueryJobs(string queue)
        {
            var jf = new JobFactory(RegistryContext);

            jf.JobDefinitionGuids.UnionWith(JobFactory.QueryJobDefinitionGuids);

            return jf.SelectJobs(-1, -1).Where(j => j is QueryJob).Cast<QueryJob>();
        }

        public QueryJob GetQueryJob(string queue, string guid)
        {
            throw new NotImplementedException();
        }

        public ExportJob[] GetExportJobs(string queue)
        {
            throw new NotImplementedException();
        }

        public ExportJob GetExportJob(string queue, string guid)
        {
            throw new NotImplementedException();
        }

        public ImportJob[] GetImportJobs(string queue)
        {
            throw new NotImplementedException();
        }

        public ImportJob GetImportJob(string queue, string guid)
        {
            throw new NotImplementedException();
        }
    }
}