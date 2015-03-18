using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Xml;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.Web.UI;

namespace Jhu.Graywulf.Web.Api.V1
{
    [KnownType(typeof(ExportJob))]
    [KnownType(typeof(ImportJob))]
    [KnownType(typeof(QueryJob))]
    [DataContract]
    [Description("Represents a job.")]
    public class Job
    {
        #region Private member variables

        /// <summary>
        /// Holds a reference to the detailed job instance
        /// registry object, if any
        /// </summary>
        private JobInstance jobInstance;

        private Guid guid;
        private string name;
        private JobType jobType;
        private JobStatus status;
        private bool canCancel;
        private JobQueue queue;
        private string comments;
        private string error;
        private DateTime? dateCreated;
        private DateTime? dateStarted;
        private DateTime? dateFinished;

        private JobDependency[] dependencies;

        #endregion
        #region Properties

        [IgnoreDataMember]
        public JobInstance JobInstance
        {
            get { return jobInstance; }
            protected set { jobInstance = value; }
        }

        [DataMember(Name = "guid")]
        [Description("Unique identifier of the job.")]
        public Guid Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        [DataMember(Name = "name")]
        [Description("Name of the job.")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [IgnoreDataMember]
        public JobType Type
        {
            get { return jobType; }
            set { jobType = value; }
        }

        [IgnoreDataMember]
        public JobStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        [DataMember(Name = "status")]
        [Description("Execution status of the job.")]
        public string Status_ForXml
        {
            get { return Util.EnumFormatter.ToXmlString(Status); }
            set { Status = Util.EnumFormatter.FromXmlString<JobStatus>(value); }
        }

        [DataMember(Name = "canCancel")]
        [Description("True, if the job is in a state which support cancelation.")]
        public bool CanCancel
        {
            get { return canCancel; }
            set { canCancel = value; }
        }

        [IgnoreDataMember]
        public JobQueue Queue
        {
            get { return queue; }
            set { queue = value; }
        }

        [DataMember(Name = "queue")]
        [Description("Name of the queue this job is scheduled in.")]
        public string Queue_ForXml
        {
            get { return Util.EnumFormatter.ToXmlString(Queue); }
            set { Queue = Util.EnumFormatter.FromXmlString<JobQueue>(value); }
        }

        [DataMember(Name = "comments", EmitDefaultValue = false)]
        [DefaultValue("")]
        [Description("User-defined comments.")]
        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }

        [DataMember(Name = "error", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Description of the error in case of a failed job.")]
        public string Error
        {
            get { return error; }
            set { error = value; }
        }

        [IgnoreDataMember]
        public DateTime? DateCreated
        {
            get { return dateCreated; }
            set { dateCreated = value; }
        }

        [DataMember(Name = "dateCreated", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Date and time when the job was submited.")]
        public string DateCreated_ForXml
        {
            get { return Util.DateFormatter.ToXmlString(DateCreated); }
            set { DateCreated = Util.DateFormatter.FromXmlString(value); }
        }

        [IgnoreDataMember]
        public DateTime? DateStarted
        {
            get { return dateStarted; }
            set { dateStarted = value; }
        }

        [DataMember(Name = "dateStarted", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Date and time when execution of the job started.")]
        public string DateStarted_ForXml
        {
            get { return Util.DateFormatter.ToXmlString(DateStarted); }
            set { DateStarted = Util.DateFormatter.FromXmlString(value); }
        }

        [IgnoreDataMember]
        public DateTime? DateFinished
        {
            get { return dateFinished; }
            set { dateFinished = value; }
        }

        [DataMember(Name = "dateFinished", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Date and time when the execution of the job finished.")]
        public string DateFinished_ForXml
        {
            get { return Util.DateFormatter.ToXmlString(DateFinished); }
            set { DateFinished = Util.DateFormatter.FromXmlString(value); }
        }

        [DataMember(Name = "dependencies", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("A list of predecessor jobs on which this job depends.")]
        public JobDependency[] Dependencies
        {
            get { return dependencies; }
            set { dependencies = value; }
        }

        #endregion
        #region Constructors and initializers

        public Job()
        {
            InitializeMembers();
        }

        public Job(Job old)
        {
            CopyMembers(old);
        }

        public static Job FromJobInstance(JobInstance jobInstance)
        {
            var job = new Job();
            job.LoadFromRegistryObject(jobInstance);

            return job;
        }

        private void InitializeMembers()
        {
            this.guid = Guid.Empty;
            this.name = null;
            this.jobType = JobType.Unknown;
            this.status = JobStatus.Unknown;
            this.canCancel = false;
            this.queue = JobQueue.Unknown;
            this.comments = String.Empty;
            this.error = null;
            this.dateCreated = null;
            this.dateStarted = null;
            this.dateFinished = null;

            this.dependencies = null;
        }

        private void CopyMembers(Job old)
        {
            this.guid = old.guid;
            this.name = old.name;
            this.jobType = old.jobType;
            this.status = old.status;
            this.canCancel = old.canCancel;
            this.queue = old.queue;
            this.comments = old.comments;
            this.error = old.error;
            this.dateCreated = old.dateCreated;
            this.dateStarted = old.dateStarted;
            this.dateFinished = old.dateFinished;

            this.dependencies = Jhu.Graywulf.Util.DeepCloner.CloneArray(old.dependencies);
        }

        #endregion

        protected virtual void LoadFromRegistryObject(JobInstance jobInstance)
        {
            this.jobInstance = jobInstance;

            this.guid = jobInstance.Guid;
            this.name = jobInstance.Name;
            this.canCancel = jobInstance.CanCancel;
            this.comments = jobInstance.Comments;
            this.error = jobInstance.ExceptionMessage;
            this.dateCreated = jobInstance.DateCreated == DateTime.MinValue ? (DateTime?)null : jobInstance.DateCreated;
            this.dateStarted = jobInstance.DateStarted == DateTime.MinValue ? (DateTime?)null : jobInstance.DateStarted;
            this.dateFinished = jobInstance.DateFinished == DateTime.MinValue ? (DateTime?)null : jobInstance.DateFinished;

            // To keep REST API entirely isolated, we need to copy
            // the enum using a switch
            switch (jobInstance.JobExecutionStatus)
            {
                case JobExecutionState.Cancelled:
                case JobExecutionState.Cancelling:
                case JobExecutionState.CancelRequested:
                    this.status = JobStatus.Canceled;
                    break;
                case JobExecutionState.Completed:
                    this.status = JobStatus.Completed;
                    break;
                case JobExecutionState.Executing:
                case JobExecutionState.Resumed:
                    this.status = JobStatus.Executing;
                    break;
                case JobExecutionState.Failed:
                    this.status = JobStatus.Failed;
                    break;
                case JobExecutionState.Persisted:
                case JobExecutionState.Persisting:
                case JobExecutionState.Scheduled:
                case JobExecutionState.Starting:
                case JobExecutionState.Suspended:
                    this.status = JobStatus.Waiting;
                    break;
                case JobExecutionState.TimedOut:
                    this.status = JobStatus.TimedOut;
                    break;
                case JobExecutionState.Unknown:
                default:
                    this.status = JobStatus.Unknown;
                    break;
            }

            // Here we make the assumption that the queue is named the same as
            // the queue definition
            var jobFactory = new JobFactory(jobInstance.Context);
            var qi = jobFactory.GetQueueInstance(jobInstance.ParentReference.Guid);

            switch (qi.Name)
            {
                case Jhu.Graywulf.Registry.Constants.QuickQueueDefinitionName:
                    this.queue = JobQueue.Quick;
                    break;
                case Jhu.Graywulf.Registry.Constants.LongQueueDefinitionName:
                    this.queue = JobQueue.Long;
                    break;
                default:
                    this.queue = JobQueue.Unknown;
                    break;
            }

            // Load job dependencies, if requested
            if (jobInstance.Dependencies != null && jobInstance.Dependencies.Count > 0)
            {
                this.dependencies = new JobDependency[jobInstance.Dependencies.Count];
                int q = 0;
                foreach (var dep in jobInstance.Dependencies.Values)
                {
                    this.dependencies[q] = new JobDependency(dep);
                }
            }
        }

        public virtual bool Validate()
        {
            throw new NotImplementedException();
        }

        public virtual void Schedule(FederationContext context)
        {
            throw new NotImplementedException();
        }

        protected virtual void SaveDependencies()
        {
            if (dependencies != null)
            {
                foreach (var dep in dependencies)
                {
                    var jd = dep.CreateRegistryObject(jobInstance);
                    jd.Save();
                }
            }
        }

        protected string GetQueueName(FederationContext context)
        {
            string queuename = null;

            switch (Queue)
            {
                case JobQueue.Quick:
                    queuename = Jhu.Graywulf.Registry.Constants.QuickQueueName;
                    break;
                case JobQueue.Long:
                    queuename = Jhu.Graywulf.Registry.Constants.LongQueueName;
                    break;
                default:
                    throw new NotImplementedException();
            }

            queuename = EntityFactory.CombineName(
                EntityType.QueueInstance,
                context.Federation.ControllerMachine.GetFullyQualifiedName(),
                queuename);

            return queuename;
        }
    }
}
