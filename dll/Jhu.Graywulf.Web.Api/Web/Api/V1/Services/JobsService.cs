using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.ServiceModel.Security;
using System.Security.Permissions;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.Query;
using Jhu.Graywulf.Web.Services;

namespace Jhu.Graywulf.Web.Api.V1
{
    [ServiceContract]
    public interface IJobsService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/queues")]
        [Description("Returns a list of all available job queues.")]
        QueueListResponse ListQueues();

        [OperationContract]
        [WebGet(UriTemplate = "/queues/{queue}")]
        QueueResponse GetQueue(string queue);

        [OperationContract]
        [WebGet(UriTemplate = "/queues/{queue}/jobs?type={type}&from={from}&max={max}")]
        JobListResponse ListJobs(string queue, string type, string from, string max);

        [OperationContract]
        [WebInvoke(Method = HttpMethod.Post, UriTemplate = "/queues/{queue}/jobs")]
        JobResponse SubmitJob(string queue, JobRequest job);

        [OperationContract]
        [WebGet(UriTemplate = "/jobs/{guid}")]
        JobResponse GetJob(string guid);

        [OperationContract]
        [WebInvoke(Method = HttpMethod.Delete, UriTemplate = "/jobs/{guid}")]
        JobResponse CancelJob(string guid);
    }

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
    [RestServiceBehavior]
    public class JobsService : RestServiceBase, IJobsService
    {
        #region Constructors and initializers

        public JobsService()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        #endregion

        public QueueListResponse ListQueues()
        {
            var jobFactory = new JobFactory(RegistryContext);

            return new QueueListResponse(jobFactory.SelectQueue());
        }

        public QueueResponse GetQueue(string queue)
        {
            var jobFactory = new JobFactory(RegistryContext);

            return new QueueResponse(jobFactory.GetQueue(queue));
        }

        public JobListResponse ListJobs(string queue, string type, string from, string max)
        {
            var jobFactory = new JobFactory(RegistryContext);

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
                    jobFactory.QueueInstanceGuids.Add(jobFactory.GetQueue(queue).Guid);
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
                    jobFactory.JobDefinitionGuids.UnionWith(jobFactory.SelectJobDefinitions(jobType).Select(jd => jd.Guid));
                    break;
                default:
                    throw new NotImplementedException();
            }

            // TODO: add options like: top, date limits, etc.
            int f = from == null ? 0 : int.Parse(from);
            int m = max == null ? 10 : int.Parse(max);

            return new JobListResponse(jobFactory.SelectJobs(f, Math.Min(m, 50)));
        }

        public JobResponse GetJob(string guid)
        {
            var jobFactory = new JobFactory(RegistryContext);

            return new JobResponse(jobFactory.GetJob(new Guid(guid)));
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

        public JobResponse CancelJob(string guid)
        {
            throw new NotImplementedException();
        }
    }
}