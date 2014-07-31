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
        [WebGet(UriTemplate = "/queues/{queue}/jobs?type={type}")]
        JobList ListJobs(string queue, string type);

        [OperationContract]
        [DynamicResponseFormat]
        [WebGet(UriTemplate = "/queues/all/jobs/{guid}")]
        Job GetJob(string guid);

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
                    // Make sure that searches are always limited to the current user
                    jobFactory = new JobFactory(RegistryContext)
                    {
                        UserGuid = RegistryContext.UserGuid
                    };
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

        public JobList ListJobs(string queue, string type)
        {
            JobQueue jobQueue;
            if (!Enum.TryParse<JobQueue>(queue, true, out jobQueue))
            {
                throw new ResourceNotFoundException();
            }

            switch (jobQueue)
            {
                case JobQueue.All:
                    // No need to change job factory settings
                    break;
                case JobQueue.Quick:
                case JobQueue.Long:
                    JobFactory.QueueInstanceGuids.Add(JobFactory.GetQueueInstance(queue).Guid);
                    break;
                default:
                    throw new NotImplementedException();
            }

            JobType jobType = JobType.All;
            if (type != null && !Enum.TryParse<JobType>(type, true, out jobType))
            {
                throw new ResourceNotFoundException();
            }

            switch (jobType)
            {
                case JobType.All:
                    // No need to change job factory settings
                    break;
                case JobType.Export:
                case JobType.Import:
                case JobType.Query:
                    JobFactory.JobDefinitionGuids.UnionWith(JobFactory.SelectJobDefinitions(jobType).Select(jd => jd.Guid));
                    break;
                default:
                    throw new NotImplementedException();
            }
            
            // TODO: add options like: top, date limits, etc.

            return new JobList(JobFactory.SelectJobs(-1, -1));
        }

        public Job GetJob(string guid)
        {
            throw new NotImplementedException();
        }

        public Job SubmitJob(string queue, Job job)
        {
            throw new NotImplementedException();
        }
    }
}