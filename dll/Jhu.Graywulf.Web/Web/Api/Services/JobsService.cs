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
    public interface IJobsService
    {
        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/queues")]
        QueueList ListQueues();

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/queues/{queue}")]
        Queue GetQueue(string queue);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/queues/{queue}/jobs")]
        JobList ListJobs(string queue);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/queues/{queue}/jobs?type={type}")]
        JobList ListJobsByType(string queue, string type);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/queues/{queue}/jobs/{guid}")]
        Job GetJob(string queue, string guid);

        [OperationContract]
        [DynamicResponseFormat]
        [WebInvoke(Method = HttpMethod.Post, UriTemplate = "/queues/{queue}/jobs")]
        Job SubmitJob(string queue, Job job);
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
    [RestServiceBehavior]
    public class JobsService : ServiceBase, IJobsService
    {
        private JobFactory jobFactory;

        #region Properties

        private JobFactory JobFactory
        {
            get
            {
                if (jobFactory == null)
                {
                    jobFactory = new JobFactory(RegistryContext);
                }

                return jobFactory;
            }
        }

        #endregion
        #region Constructors and initializers

        public JobsService()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.jobFactory = null;
        }

        #endregion

        public QueueList ListQueues()
        {
            return new QueueList(JobFactory.SelectQueueInstances());
        }

        public Queue GetQueue(string queue)
        {
            return new Queue(JobFactory.GetQueueInstance(queue));
        }

        public JobList ListJobs(string queue)
        {
            if (Entity.StringComparer.Compare(queue, "any") != 0)
            {
                // If a specific queue is to be returned, set the appropriate guid
                JobFactory.QueueInstanceGuid.Add(JobFactory.GetQueueInstance(queue).Guid);
            }
            
            // TODO: add options like: top, date limits, etc.

            return new JobList(JobFactory.SelectJobs(-1, -1));
        }

        public JobList ListJobsByType(string queue, string type)
        {
            throw new NotImplementedException();
        }

        public Job GetJob(string queue, string guid)
        {
            throw new NotImplementedException();
        }

        public Job SubmitJob(string queue, Job job)
        {
            throw new NotImplementedException();
        }

#if false
        public IEnumerable<QueryJob> GetQueryJobs(string queue)
        {
            var jf = new JobFactory(RegistryContext);

            jf.JobDefinitionGuids.UnionWith(JobFactory.QueryJobDefinitionGuids);

            return jf.SelectJobs(-1, -1).Where(j => j is QueryJob).Cast<QueryJob>();
        }
#endif
    }
}