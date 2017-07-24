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
    internal class Queue
    {
        private Guid guid;
        private int maxOutstandingJobs;
        private ConcurrentDictionary<Guid, Job> jobs;
        private TimeSpan timeout;
        private Guid lastUserGuid;

        /// <summary>
        /// Unique ID of the queue, equals to QueueInstance.Guid
        /// </summary>
        public Guid Guid
        {
            get { return guid; }
        }

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

        public Queue()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.guid = Guid.Empty;
            this.maxOutstandingJobs = -1;
            this.jobs = new ConcurrentDictionary<Guid, Job>();
            this.timeout = TimeSpan.Zero;
            this.lastUserGuid = Guid.Empty;
        }

        /// <summary>
        /// Updates the queue information based on the values
        /// read from the registry.
        /// </summary>
        /// <param name="q"></param>
        public void Update(QueueInstance q)
        {
            this.guid = q.Guid;
            this.maxOutstandingJobs = q.MaxOutstandingJobs;
            this.timeout = TimeSpan.FromSeconds(q.Timeout);
        }
    }
}
