using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.UI.Api
{
    [KnownType(typeof(ExportJob))]
    [KnownType(typeof(ImportJob))]
    [KnownType(typeof(QueryJob))]
    [DataContract]
    public class Job
    {
        private Guid guid;
        private string name;
        private JobStatus status;
        private bool canCancel;
        private JobQueue queue;
        private string comments;
        private string error;
        private DateTime? dateCreated;
        private DateTime? dateStarted;
        private DateTime? dateFinished;

        public Guid Guid
        {
            get { return guid; }
            set { guid = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public virtual JobType Type
        {
            get { return JobType.Unknown; }
            set { }
        }

        public JobStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        public bool CanCancel
        {
            get { return canCancel; }
            set { canCancel = value; }
        }

        public JobQueue Queue
        {
            get { return queue; }
            set { queue = value; }
        }

        public string Comments
        {
            get { return comments; }
            set { comments = value; }
        }

        public string Error
        {
            get { return error; }
            set { error = value; }
        }

        public DateTime? DateCreated
        {
            get { return dateCreated; }
            set { dateCreated = value; }
        }

        public DateTime? DateStarted
        {
            get { return dateStarted; }
            set { dateStarted = value; }
        }

        public DateTime? DateFinished
        {
            get { return dateFinished; }
            set { dateFinished = value; }
        }

        public Job()
        {
            InitializeMembers();
        }

        public Job(JobInstance jobInstance)
        {
            InitializeMembers();

            CopyFromJobInstance(jobInstance);
        }

        private void InitializeMembers()
        {
            this.guid = Guid.Empty;
            this.name = null;
            this.status = JobStatus.Unknown;
            this.canCancel = false;
            this.queue = JobQueue.Unknown;
            this.comments = String.Empty;
            this.error = null;
            this.dateCreated = null;
            this.dateStarted = null;
            this.dateFinished = null;
        }

        private void CopyFromJobInstance(JobInstance jobInstance)
        {
            this.guid = jobInstance.Guid;
            this.name = jobInstance.Name;
            this.canCancel = jobInstance.CanCancel;
            this.comments = jobInstance.Comments;
            this.error = jobInstance.ExceptionMessage;
            this.dateCreated = jobInstance.DateCreated == DateTime.MinValue ? (DateTime?)null : jobInstance.DateCreated;
            this.dateStarted = jobInstance.DateStarted == DateTime.MinValue ? (DateTime?)null : jobInstance.DateStarted;
            this.dateFinished = jobInstance.DateFinished == DateTime.MinValue ? (DateTime?)null : jobInstance.DateFinished;

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

            switch (jobInstance.QueueInstance.QueueDefinition.Name)
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
        }

        protected string GetXmlInnerText(XmlDocument xml, string path)
        {
            return GetXmlInnerText(xml.ChildNodes, path.Split('/'), 0);
        }

        private string GetXmlInnerText(XmlNodeList nodes, string[] path, int i)
        {
            for (int k = 0; k < nodes.Count; k++)
            {
                var n = nodes[k];
                if (StringComparer.InvariantCultureIgnoreCase.Compare(n.LocalName, path[i]) == 0)
                {
                    if (i == path.Length - 1)
                    {
                        return n.InnerText;
                    }
                    else
                    {
                        return GetXmlInnerText(n.ChildNodes, path, i + 1);
                    }
                }
            }

            throw new KeyNotFoundException();
        }

        public virtual JobInstance Schedule(FederationContext context)
        {
            throw new NotImplementedException();
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
