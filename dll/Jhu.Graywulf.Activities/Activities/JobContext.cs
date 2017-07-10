using System;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Activities
{
    [Serializable]
    public class JobContext : MarshalByRefObject, ICloneable
    {
        #region Private member variables

        private Guid clusterGuid;
        private Guid domainGuid;
        private Guid federationGuid;
        private Guid jobGuid;
        private Guid userGuid;
        private string userName;
        private string jobID;

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

        public string JobID
        {
            get { return jobID; }
            set { jobID = value; }
        }

        #endregion

        #region Constructors and initializers

        public JobContext()
        {
            InitializeMembers(new StreamingContext());
        }

        public JobContext(JobContext old)
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
            this.userGuid = Guid.Empty;
            this.userName = null;
            this.jobID = null;
        }

        private void CopyMembers(JobContext old)
        {
            this.clusterGuid = old.clusterGuid;
            this.domainGuid = old.domainGuid;
            this.federationGuid = old.federationGuid;
            this.jobGuid = old.jobGuid;
            this.userGuid = old.userGuid;
            this.userName = old.userName;
            this.jobID = old.jobID;
        }
        
        public JobContext Clone()
        {
            return new JobContext(this);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
