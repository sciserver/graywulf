using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Scheduler
{
    /// <summary>
    /// Represents a running job
    /// </summary>
    [Serializable]
    public class Job : MarshalByRefObject
    {
        private Guid guid;
        private string jobID;
        private Guid userGuid;
        private string userName;
        private int appDomainID;
        private Guid queueGuid;
        private string workflowTypeName;
        private Guid workflowInstanceId;
        private DateTime timeStarted;
        private JobStatus status;

        /// <summary>
        /// Unique ID, equals to the JobInstance.Guid.
        /// </summary>
        public Guid Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        public string JobID
        {
            get { return jobID; }
            set { jobID = value; }
        }

        public Guid UserGuid
        {
            get { return userGuid; }
            set { userGuid = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
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

        public bool IsTimedOut(TimeSpan timeout)
        {
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
            this.guid = Guid.Empty;
            this.jobID = null;
            this.userGuid = Guid.Empty;
            this.userName = null;
            this.appDomainID = -1;
            this.queueGuid = Guid.Empty;
            this.workflowTypeName = null;
            this.workflowInstanceId = Guid.Empty;
            this.timeStarted = DateTime.MinValue;
            this.status = JobStatus.Unknown;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
