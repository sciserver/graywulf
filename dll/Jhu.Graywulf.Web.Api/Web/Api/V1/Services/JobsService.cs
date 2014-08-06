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
    [Description("Manages queues and jobs.")]
    public interface IJobsService
    {
        [OperationContract]
        [WebGet(UriTemplate = "/queues")]
        [Description("Returns a list of all available job queues.")]
        QueueListResponse ListQueues();

        [OperationContract]
        [WebGet(UriTemplate = "/queues/{queue}")]
        [Description("Returns information about a job queue.")]
        QueueResponse GetQueue(
            [Description("Name of the queue.")]
            string queue);

        [OperationContract]
        [WebGet(UriTemplate = "/queues/{queue}/jobs?type={type}&from={from}&max={max}")]
        [Description("Lists the jobs in the queue in descending order by submission time. Only jobs of the authenticated user are listed.")]
        JobListResponse ListJobs(
            [Description("Name of the queue.")]
            string queue,
            [Description("Type of the job.")]
            string type,
            [Description("First job in the queue to be returned.")]
            string from,
            [Description("Maximum number of jobs to be returned.")]
            string max);

        [OperationContract]
        [WebInvoke(Method = HttpMethod.Post, UriTemplate = "/queues/{queue}/jobs")]
        [Description("Submits a new job of any kind.")]
        JobResponse SubmitJob(
            [Description("Name of the queue.")]
            string queue,
            [Description("The job to be sceduled.")]
            JobRequest job);

        [OperationContract]
        [WebGet(UriTemplate = "/jobs/{guid}")]
        [Description("Returns a single job.")]
        JobResponse GetJob(
            [Description("Unique ID of the job to be returned.")]
            string guid);

        [OperationContract]
        [WebInvoke(Method = HttpMethod.Delete, UriTemplate = "/jobs/{guid}")]
        [Description("Cancels a single job.")]
        JobResponse CancelJob(
            [Description("Unique ID of the job to be canceled..")]
            string guid);
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
            var q = jobFactory.GetQueue(queue);

            q.PendingJobCount = jobFactory.CountPendingJobs(q.Name);

            return new QueueResponse(q);
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
            // Load job
            var jobFactory = new JobFactory(RegistryContext);

            return new JobResponse(jobFactory.CancelJob(new Guid(guid)));
        }
    }
}