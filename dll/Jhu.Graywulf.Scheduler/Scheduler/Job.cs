using System;
using System.Runtime.Serialization;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Scheduler
{
    /// <summary>
    /// Represents a running job
    /// </summary>
    [Serializable]
    internal class Job : JobInfo
    {
        private int appDomainID;
        private Guid queueGuid;
        private string workflowTypeName;
        private Guid workflowInstanceId;
        private DateTime timeStarted;
        private TimeSpan timeout;
        private JobStatus status;

        /// <summary>
        /// Unique ID, equals to the JobInstance.Guid.
        /// </summary>
        public Guid Guid
        {
            get { return JobGuid; }
            set { JobGuid = value; }
        }

        /// <summary>
        /// ID of the AppDomain used to execute the job.
        /// </summary>
        public int AppDomainID
        {
            get { return appDomainID; }
            set { appDomainID = value; }
        }

        /// <summary>
        /// ID of the queue the job is queued in, equals to QueueInstance.Guid
        /// </summary>
        public Guid QueueGuid
        {
            get { return queueGuid; }
            set { queueGuid = value; }
        }

        public string WorkflowTypeName
        {
            get { return workflowTypeName; }
            set { workflowTypeName = value; }
        }

        /// <summary>
        /// ID of the workflow associated with the running job.
        /// </summary>
        public Guid WorkflowInstanceId
        {
            get { return workflowInstanceId; }
            set { workflowInstanceId = value; }
        }

        /// <summary>
        /// Start time of the job.
        /// </summary>
        /// <remarks>
        /// This value is used to determine if the job has timed out.
        /// </remarks>
        public DateTime TimeStarted
        {
            get { return timeStarted; }
            set { timeStarted = value; }
        }

        public TimeSpan Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        /// <summary>
        /// Last known execution status of the job.
        /// </summary>
        /// <remarks>
        /// This is not the same as the execution status reported in the registry and
        /// not the status of the workflow. Mainly used by the poller to queue newly
        /// picked up jobs for execution and to report time outs.
        /// </remarks>
        public JobStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        public bool IsTimedOut(TimeSpan queueTimeout)
        {
            TimeSpan timeout;

            if (this.timeout == TimeSpan.Zero)
            {
                timeout = queueTimeout;
            }
            else if (queueTimeout == TimeSpan.Zero)
            {
                timeout = this.timeout;
            }
            else
            {
                timeout = this.timeout <= queueTimeout ? this.timeout : queueTimeout;
            }

            var runtime = DateTime.Now - timeStarted;

            return runtime > timeout &&
                   timeout != TimeSpan.Zero &&
                   status == JobStatus.Executing;
        }

        public Job()
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.appDomainID = -1;
            this.queueGuid = Guid.Empty;
            this.workflowTypeName = null;
            this.workflowInstanceId = Guid.Empty;
            this.timeStarted = DateTime.MinValue;
            this.timeout = TimeSpan.Zero;
            this.status = JobStatus.Unknown;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
