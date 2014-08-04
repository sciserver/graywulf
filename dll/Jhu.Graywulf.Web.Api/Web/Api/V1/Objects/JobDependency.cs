using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Xml;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract(Name = "jobDependency")]
    public class JobDependency : ICloneable
    {
        #region Private member variables

        private Guid predecessorJobGuid;
        private JobDependencyCondition condition;

        #endregion
        #region Properties

        [DataMember(Name = "predecessorJob")]
        public Guid PredecessorJobGuid
        {
            get { return predecessorJobGuid; }
            set { predecessorJobGuid = value; }
        }

        [IgnoreDataMember]
        public JobDependencyCondition Condition
        {
            get { return condition; }
            set { condition = value; }
        }

        [DataMember(Name = "condition")]
        public string Condition_ForXml
        {
            get { return Util.EnumFormatter.ToXmlString(condition); }
            set { condition = Util.EnumFormatter.FromXmlString<JobDependencyCondition>(value); }
        }

        #endregion
        #region Constructors and initializers

        public JobDependency()
        {
            InitializeMembers();
        }

        public JobDependency(JobDependency old)
        {
            CopyMembers(old);
        }

        public JobDependency(JobInstanceDependency jobInstanceDependency)
        {
            InitializeMembers();

            CopyFromJobInstanceDependency(jobInstanceDependency);
        }

        private void InitializeMembers()
        {
            this.predecessorJobGuid = Guid.Empty;
            this.condition = JobDependencyCondition.Unknown;
        }

        private void CopyMembers(JobDependency old)
        {
            this.predecessorJobGuid = old.predecessorJobGuid;
            this.condition = old.condition;
        }

        private void CopyFromJobInstanceDependency(JobInstanceDependency jobInstanceDependency)
        {
            this.predecessorJobGuid = jobInstanceDependency.PredecessorJobInstanceReference.Guid;

            // To keep REST API entirely isolated, we need to copy
            // the enum using a switch
            switch (jobInstanceDependency.Condition)
            {
                case Registry.JobDependencyCondition.Unknown:
                    this.condition = JobDependencyCondition.Unknown;
                    break;
                case Registry.JobDependencyCondition.Completed:
                    this.condition = JobDependencyCondition.Completed;
                    break;
                case Registry.JobDependencyCondition.Cancelled:
                    this.condition = JobDependencyCondition.Cancelled;
                    break;
                case Registry.JobDependencyCondition.Failed:
                    this.condition = JobDependencyCondition.Failed;
                    break;
                case Registry.JobDependencyCondition.TimedOut:
                    this.condition = JobDependencyCondition.TimedOut;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public object Clone()
        {
            return new JobDependency(this);
        }

        #endregion
    }
}
