using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Scheduler
{
    /// <summary>
    /// Represents a job queue
    /// </summary>
    public class Queue
    {
        private Guid guid;
        private Dictionary<Guid, Job> jobs;
        private TimeSpan timeout;

        /// <summary>
        /// Unique ID of the queue, equals to QueueInstance.Guid
        /// </summary>
        public Guid Guid
        {
            get { return guid; }
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
        /// Job timeout interval in this queue.
        /// </summary>
        public TimeSpan Timeout
        {
            get { return timeout; }
        }

        public Queue()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.guid = Guid.Empty;
            this.jobs = new Dictionary<Guid, Job>();
            this.timeout = TimeSpan.Zero;
        }

        /// <summary>
        /// Updates the queue information based on the values
        /// read from the registry.
        /// </summary>
        /// <param name="q"></param>
        public void Update(QueueInstance q)
        {
            this.guid = q.Guid;
            this.timeout = TimeSpan.FromSeconds(q.Timeout);
        }
    }
}
