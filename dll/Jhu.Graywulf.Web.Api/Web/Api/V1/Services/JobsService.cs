using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.ServiceModel.Security;
using System.Security.Permissions;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;

namespace Jhu.Graywulf.Web.Api.V1
{
    [ServiceContract]
    public interface IJobsService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/queues")]
        QueueListResponse ListQueues();

        [OperationContract]
        [WebGet(UriTemplate = "/queues/{queue}")]
        QueueResponse GetQueue(string queue);

        [OperationContract]
        [WebGet(UriTemplate = "/queues/{queue}/jobs?type={type}")]
        JobListResponse ListJobs(string queue, string type);

        [OperationContract]
        [WebGet(UriTemplate = "/queues/all/jobs/{guid}")]
        JobResponse GetJob(string guid);

        [OperationContract]
        [WebInvoke(Method = HttpMethod.Post, UriTemplate = "/queues/{queue}/jobs")]
        JobResponse SubmitJob(string queue, JobRequest job);
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
    [RestServiceBehavior]
    public class JobsService : RestServiceBase, IJobsService
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

        public QueueListResponse ListQueues()
        {
            return new QueueListResponse(JobFactory.SelectQueue());
        }

        public QueueResponse GetQueue(string queue)
        {
            return new QueueResponse(JobFactory.GetQueue(queue));
        }

        public JobListResponse ListJobs(string queue, string type)
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
                    JobFactory.QueueInstanceGuids.Add(JobFactory.GetQueue(queue).Guid);
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

            return new JobListResponse(JobFactory.SelectJobs(-1, -1));
        }

        public JobResponse GetJob(string guid)
        {
            return new JobResponse(JobFactory.GetJob(new Guid(guid)));
        }

        public JobResponse SubmitJob(string queue, JobRequest jobRequest)
        {
            var job = jobRequest.GetValue();

            // TODO: do some more scrubbing of the submitted job object here
            // if necessary, prior to scheduling it as a real job

            job.Queue = Util.EnumFormatter.FromXmlString<JobQueue>(queue);

            job.Schedule(FederationContext);
            return new JobResponse(job);
        }
    }
}