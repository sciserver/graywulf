using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Scheduler
{
    /// <summary>
    /// Represents a job queue
    /// </summary>
    [Serializable]
    public class Queue : RegistryObject
    {
        private int maxOutstandingJobs;
        private Dictionary<Guid, Job> jobs;
        private TimeSpan timeout;
        private Guid lastUserGuid;

        /// <summary>
        /// Gets the maximum jobs that can be executed in parallel from
        /// the queue.
        /// </summary>
        public int MaxOutstandingJobs
        {
            get { return maxOutstandingJobs; }
        }

        /// <summary>
        /// Jobs associated with the queue and already loaded by
        /// the poller.
        /// </summary>
        public Dictionary<Guid, Job> Jobs
        {
            get { return jobs; }
        }

        /// <summary>
        /// Gets the job timeout period of the queue.
        /// </summary>
        public TimeSpan Timeout
        {
            get { return timeout; }
        }

        /// <summary>
        /// Gets the GUID of the user owning the last scheduled job.
        /// </summary>
        public Guid LastUserGuid
        {
            get { return lastUserGuid; }
            set { lastUserGuid = value; }
        }
        
        public Queue(QueueInstance qi) :
            base (qi)
        {
            InitializeMembers();

            this.maxOutstandingJobs = qi.MaxOutstandingJobs;
            this.timeout = TimeSpan.FromSeconds(qi.Timeout);
        }

        private void InitializeMembers()
        {
            this.maxOutstandingJobs = -1;
            this.jobs = new Dictionary<Guid, Job>();
            this.timeout = TimeSpan.Zero;
            this.lastUserGuid = Guid.Empty;
        }
    }
}
