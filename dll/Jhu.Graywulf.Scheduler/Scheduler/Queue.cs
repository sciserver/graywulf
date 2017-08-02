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
    internal class Queue : RegistryObject, IQueue
    {
        private int maxOutstandingJobs;
        private ConcurrentDictionary<Guid, Job> jobs;
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
        public ConcurrentDictionary<Guid, Job> Jobs
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

        int IQueue.JobCount
        {
            get { return jobs.Count; }
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
            this.jobs = new ConcurrentDictionary<Guid, Job>();
            this.timeout = TimeSpan.Zero;
            this.lastUserGuid = Guid.Empty;
        }
    }
}
