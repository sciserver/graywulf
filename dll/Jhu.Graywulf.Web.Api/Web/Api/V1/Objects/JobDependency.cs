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
    [Description("Represents a dependency between to jobs")]
    public class JobDependency : ICloneable
    {
        #region Private member variables

        private Guid predecessorJobGuid;
        private JobDependencyCondition condition;

        #endregion
        #region Properties

        [DataMember(Name = "predecessorJob")]
        [Description("Unique ID of the predecessor job.")]
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
        [Description("Condition on the predecessor job has to met before this job is executed.")]
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

            LoadFromRegistryObject(jobInstanceDependency);
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

        public object Clone()
        {
            return new JobDependency(this);
        }

        #endregion

        private void LoadFromRegistryObject(JobInstanceDependency jobInstanceDependency)
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

        internal JobInstanceDependency CreateRegistryObject(JobInstance jobInstance)
        {
            var ef = new EntityFactory(jobInstance.Context);
            var predji = ef.LoadEntity<JobInstance>(predecessorJobGuid);

            var jd = new JobInstanceDependency(jobInstance);

            jd.Name = String.Format("{0}_{1}", jobInstance.Name, predji.Name);

            jd.PredecessorJobInstanceReference.Guid = predecessorJobGuid;

            // To keep REST API entirely isolated, we need to copy
            // the enum using a switch
            switch (condition)
            {
                case JobDependencyCondition.Unknown:
                    jd.Condition = Registry.JobDependencyCondition.Unknown;
                    break;
                case JobDependencyCondition.Completed:
                    jd.Condition = Registry.JobDependencyCondition.Completed;
                    break;
                case JobDependencyCondition.Cancelled:
                    jd.Condition = Registry.JobDependencyCondition.Cancelled;
                    break;
                case JobDependencyCondition.Failed:
                    jd.Condition = Registry.JobDependencyCondition.Failed;
                    break;
                case JobDependencyCondition.TimedOut:
                    jd.Condition = Registry.JobDependencyCondition.TimedOut;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return jd;
        }
    }
}
