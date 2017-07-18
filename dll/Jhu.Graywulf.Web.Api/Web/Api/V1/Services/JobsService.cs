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
    [ServiceName(Name = "Jobs", Version = "V1")]
    [Description("Manages queues and jobs.")]
    public interface IJobsService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "*", Method = "OPTIONS")]
        void HandleHttpOptionsRequest();

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
    [RestServiceBehavior]
    public class JobsService : RestServiceBase, IJobsService
    {
        #region Constructors and initializers

        public JobsService()
        {
            InitializeMembers();
        }

        public JobsService(FederationContext federationContext)
            : base(federationContext)
        {
        }

        private void InitializeMembers()
        {
        }

        #endregion

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public QueueListResponse ListQueues()
        {
            var jobFactory = new JobFactory(RegistryContext);

            return new QueueListResponse(jobFactory.SelectQueue());
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public QueueResponse GetQueue(string queue)
        {
            var jobFactory = new JobFactory(RegistryContext);
            var q = jobFactory.GetQueue(queue);

            q.PendingJobCount = jobFactory.CountPendingJobs(q.Name);

            return new QueueResponse(q);
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
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
                case JobType.Copy:
                    jobFactory.JobDefinitionGuids.UnionWith(jobFactory.SelectJobDefinitions(jobType).Select(jd => jd.Guid));
                    break;
                default:
                    throw new NotImplementedException();
            }

            int f = from == null ? 0 : int.Parse(from);
            int m = max == null ? 10 : int.Parse(max);

            return new JobListResponse(jobFactory.SelectJobs(f, Math.Min(m, 50)));
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public JobResponse GetJob(string guid)
        {
            var jobFactory = new JobFactory(RegistryContext);
            return new JobResponse(jobFactory.GetJob(new Guid(guid)));
        }

        public JobResponse SubmitJob(Job job)
        {
            var req = new JobRequest();
            req.SetValue(job);
            return OnSubmitJob(job);
        }

        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public JobResponse SubmitJob(string queue, JobRequest jobRequest)
        {
            var job = jobRequest.GetValue();

            // TODO: do some more scrubbing of the submitted job object here
            // if necessary, prior to scheduling it as a real job
            job.Queue = Util.EnumFormatter.FromXmlString<JobQueue>(queue);

            return OnSubmitJob(job);
        }

        private JobResponse OnSubmitJob(Job job)
        {
            job.Schedule(FederationContext, GetJobQueueName(job.Queue));

            LogOperation(job, "Scheduled new {0} {1} in queue {2}.", job.GetType().Name, job.Name, job.Queue);

            return new JobResponse(job);
        }


        [PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
        public JobResponse CancelJob(string guid)
        {
            // Load job
            var g = new Guid(guid);
            var jobFactory = new JobFactory(RegistryContext);
            var job = jobFactory.CancelJob(g);

            LogOperation(job, "Cancelled {0} {1}", job.GetType().Name, job.Name);

            return new JobResponse(job);
        }

        protected string GetJobQueueName(JobQueue queue)
        {
            string queuename = null;

            switch (queue)
            {
                case JobQueue.Quick:
                    queuename = Jhu.Graywulf.Registry.Constants.QuickQueueName;
                    break;
                case JobQueue.Long:
                    queuename = Jhu.Graywulf.Registry.Constants.LongQueueName;
                    break;
                default:
                    throw new NotImplementedException();
            }

            queuename = EntityFactory.CombineName(
                EntityType.QueueInstance,
                FederationContext.Federation.ControllerMachineRole.GetFullyQualifiedName(),
                queuename);

            return queuename;
        }

        protected JobQueue GetJobQueue(QueueInstance queueInstance)
        {
            switch (queueInstance.Name)
            {
                case Jhu.Graywulf.Registry.Constants.QuickQueueDefinitionName:
                    return JobQueue.Quick;
                case Jhu.Graywulf.Registry.Constants.LongQueueDefinitionName:
                    return JobQueue.Long;
                default:
                    return JobQueue.Unknown;
            }
        }

        protected void LogOperation(Job job, string message, params object[] args)
        {
            var method = Logging.LoggingContext.Current.UnwindStack(2);
            var operation = method.DeclaringType.FullName + "." + method.Name;
            var e = Logging.LoggingContext.Current.CreateEvent(
                Logging.EventSeverity.Operation,
                Logging.EventSource.WebService,
                String.Format(message, args),
                operation,
                null,
                null);

            e.JobGuid = job.Guid;
            e.JobName = job.Name;

            Logging.LoggingContext.Current.RecordEvent(e);
        }
    }
}