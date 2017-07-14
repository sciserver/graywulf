using System;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Activities
{
    [Serializable]
    public class JobInfo : MarshalByRefObject, ICloneable
    {
        #region Private member variables

        private Guid clusterGuid;
        private Guid domainGuid;
        private Guid federationGuid;
        private Guid jobGuid;
        private string jobID;
        private string jobName;
        private Guid userGuid;
        private string userName;

        #endregion
        #region Public properties

        public Guid ClusterGuid
        {
            get { return clusterGuid; }
            set { clusterGuid = value; }
        }

        public Guid DomainGuid
        {
            get { return domainGuid; }
            set { domainGuid = value; }
        }

        public Guid FederationGuid
        {
            get { return federationGuid; }
            set { federationGuid = value; }
        }

        public Guid JobGuid
        {
            get { return jobGuid; }
            set { jobGuid = value; }
        }

        public string JobID
        {
            get { return jobID; }
            set { jobID = value; }
        }

        public string JobName
        {
            get { return jobName; }
            set { jobName = value; }
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

        #endregion
        #region Constructors and initializers

        public JobInfo()
        {
            InitializeMembers(new StreamingContext());
        }

        public JobInfo(JobInfo old)
        {
            CopyMembers(old);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.clusterGuid = Guid.Empty;
            this.domainGuid = Guid.Empty;
            this.federationGuid = Guid.Empty;
            this.jobGuid = Guid.Empty;
            this.JobID = null;
            this.jobName = null;
            this.userGuid = Guid.Empty;
            this.userName = null;
        }

        private void CopyMembers(JobInfo old)
        {
            this.clusterGuid = old.clusterGuid;
            this.domainGuid = old.domainGuid;
            this.federationGuid = old.federationGuid;
            this.jobGuid = old.jobGuid;
            this.JobID = old.JobID;
            this.jobName = old.jobName;
            this.userGuid = old.userGuid;
            this.userName = old.userName;
        }
        
        public JobInfo Clone()
        {
            return new JobInfo(this);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion

        public void UpdateLoggingContext(Logging.LoggingContext context)
        {
            context.JobGuid = this.jobGuid;
            context.JobName = this.jobName;
            context.UserGuid = this.userGuid;
            context.UserName = this.userName;
        }
    }
}
