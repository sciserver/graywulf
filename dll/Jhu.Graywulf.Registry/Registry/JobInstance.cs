/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using System.ComponentModel;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Job Instance</b> entity
    /// </summary>
    public partial class JobInstance : Entity
    {
        public enum ReferenceType : int
        {
            JobDefinition = 1,
        }

        #region Member Variables

        // --- Background storage for properties ---
        private string workflowTypeName;
        private DateTime dateStarted;
        private DateTime dateFinished;
        private JobExecutionState jobExecutionStatus;
        private DateTime suspendTimeout;
        private ScheduleType scheduleType;
        private DateTime scheduleTime;
        private RecurringPeriod recurringPeriod;
        private int recurringInterval;
        private long recurringMask;
        private Guid workflowInstanceId;
        private DateTime adminRequestTime;
        private JobAdminRequestData adminRequestData;
        private int adminRequestResult;
        private string exceptionMessage;
        private ParameterCollection parameters;

        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.JobInstance; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Jobs; }
        }

        [XmlIgnore]
        public string JobID
        {
            get { return DateCreated.ToString("yyMMddHHmmssff"); }
        }

        [DBColumn(Size = 1024)]
        public string WorkflowTypeName
        {
            get { return workflowTypeName; }
            set { workflowTypeName = value; }
        }

        [DBColumn]
        public DateTime DateStarted
        {
            get { return dateStarted; }
            set { dateStarted = value; }
        }

        [DBColumn]
        public DateTime DateFinished
        {
            get { return dateFinished; }
            set { dateFinished = value; }
        }

        [DBColumn]
        public JobExecutionState JobExecutionStatus
        {
            get { return jobExecutionStatus; }
            set { jobExecutionStatus = value; }
        }

        [DBColumn]
        public DateTime SuspendTimeout
        {
            get { return suspendTimeout; }
            set { suspendTimeout = value; }
        }

        [DBColumn]
        public ScheduleType ScheduleType
        {
            get { return scheduleType; }
            set { scheduleType = value; }
        }

        [DBColumn]
        public DateTime ScheduleTime
        {
            get { return scheduleTime; }
            set { scheduleTime = value; }
        }

        [DBColumn]
        public RecurringPeriod RecurringPeriod
        {
            get { return recurringPeriod; }
            set { recurringPeriod = value; }
        }

        [DBColumn]
        public int RecurringInterval
        {
            get { return recurringInterval; }
            set { recurringInterval = value; }
        }

        [DBColumn]
        public long RecurringMask
        {
            get { return recurringMask; }
            set { recurringMask = value; }
        }

        [DBColumn]
        public Guid WorkflowInstanceId
        {
            get { return workflowInstanceId; }
            set { workflowInstanceId = value; }
        }

        [DBColumn]
        public DateTime AdminRequestTime
        {
            get { return adminRequestTime; }
            set { adminRequestTime = value; }
        }

        [DBColumn]
        public JobAdminRequestData AdminRequestData
        {
            get { return adminRequestData; }
            set { adminRequestData = value; }
        }

        [DBColumn]
        public int AdminRequestResult
        {
            get { return adminRequestResult; }
            set { adminRequestResult = value; }
        }

        [DBColumn]
        public string ExceptionMessage
        {
            get { return exceptionMessage; }
            set { exceptionMessage = value; }
        }

        /// <summary>
        /// Gets a dictionary with the workflow input parameters.
        /// </summary>
        [XmlIgnore]
        [DBColumn]
        public ParameterCollection Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        [XmlArray("Parameters")]
        [XmlArrayItem(typeof(Parameter))]
        [DefaultValue(null)]
        public Parameter[] Parameters_ForXml
        {
            get { return parameters.GetAsArray(); }
            set { parameters = new ParameterCollection(value); }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Queue Instance</b> object to which this <b>Job Instance</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        [XmlIgnore]
        public QueueInstance QueueInstance
        {
            get { return (QueueInstance)ParentReference.Value; }
        }

        [XmlIgnore]
        public EntityReference<JobDefinition> JobDefinitionReference
        {
            get { return (EntityReference<JobDefinition>)EntityReferences[(int)ReferenceType.JobDefinition]; }
        }

        [XmlIgnore]
        public JobDefinition JobDefinition
        {
            get { return JobDefinitionReference.Value; }
            set { JobDefinitionReference.Value = value; }
        }

        [XmlElement("JobDefinition")]
        public string JobDefinition_ForXml
        {
            get { return JobDefinitionReference.Name; }
            set { JobDefinitionReference.Name = value; }
        }

        [XmlIgnore]
        public Dictionary<string, JobInstanceDependency> Dependencies
        {
            get { return GetChildren<JobInstanceDependency>(); }
            set { SetChildren<JobInstanceDependency>(value); }
        }

        #endregion
        #region Validation Properties

        [XmlIgnore]
        public bool CanCancel
        {
            get
            {
                switch (jobExecutionStatus)
                {
                    case Registry.JobExecutionState.Executing:
                    case Registry.JobExecutionState.Persisted:
                    case Registry.JobExecutionState.Scheduled:
                        return true;
                    default:
                        return false;
                }

            }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public JobInstance()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Constructor for creating a new <b>Database Definition</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public JobInstance(Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public JobInstance(QueueInstance parent)
            : base(parent.Context, parent)
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Database Definition</b> objects.
        /// </summary>
        /// <param name="old">The <b>Database Definition</b> to copy from.</param>
        public JobInstance(JobInstance old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variables to their initial values.
        /// </summary>
        /// <remarks>
        /// This function is called by the contructors.
        /// </remarks>
        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.workflowTypeName = string.Empty;
            this.dateStarted = DateTime.MinValue;
            this.dateFinished = DateTime.MinValue;
            this.jobExecutionStatus = JobExecutionState.Unknown;
            this.suspendTimeout = DateTime.MinValue;
            this.scheduleType = ScheduleType.Unknown;
            this.scheduleTime = DateTime.MinValue;
            this.recurringPeriod = RecurringPeriod.Unknown;
            this.recurringInterval = 0;
            this.recurringMask = 0;
            this.workflowInstanceId = Guid.Empty;
            this.adminRequestTime = DateTime.MinValue;
            this.adminRequestData = null;
            this.adminRequestResult = -1;
            this.exceptionMessage = null;
            this.parameters = new ParameterCollection();
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Database Definition</b> object to create the deep copy from.</param>
        private void CopyMembers(JobInstance old)
        {
            this.workflowTypeName = old.workflowTypeName;
            this.dateStarted = old.dateStarted;
            this.dateFinished = old.dateFinished;
            this.jobExecutionStatus = old.jobExecutionStatus;
            this.suspendTimeout = old.suspendTimeout;
            this.scheduleType = old.scheduleType;
            this.scheduleTime = old.scheduleTime;
            this.recurringPeriod = old.recurringPeriod;
            this.recurringInterval = old.recurringInterval;
            this.recurringMask = old.recurringMask;
            this.workflowInstanceId = old.workflowInstanceId;
            this.adminRequestTime = old.adminRequestTime;
            this.adminRequestData = old.adminRequestData == null ? null : new JobAdminRequestData(old.adminRequestData);
            this.adminRequestResult = old.adminRequestResult;
            this.exceptionMessage = old.exceptionMessage;
            this.parameters = new ParameterCollection(old.parameters);
        }

        public override object Clone()
        {
            return new JobInstance(this);
        }

        protected override IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[]
            {
                new EntityReference<JobDefinition>((int)ReferenceType.JobDefinition),
            };
        }

        protected override EntityType[] CreateChildTypes()
        {
            return new EntityType[]
            {
                EntityType.JobInstanceDependency,
            };
        }

        #endregion
        #region Cancelation functions

        /// <summary>
        /// Flags the job for cancellation.
        /// </summary>
        /// <remarks>
        /// The scheduler's poller will periodically read this value from
        /// the registry and schedule the job for cancel.
        /// </remarks>
        public void Cancel()
        {
            switch (jobExecutionStatus)
            {
                case JobExecutionState.Cancelled:
                case JobExecutionState.CancelRequested:
                case JobExecutionState.Cancelling:
                case JobExecutionState.Completed:
                case JobExecutionState.Failed:
                case JobExecutionState.TimedOut:
                case JobExecutionState.Starting:
                    throw new InvalidOperationException();
                case JobExecutionState.Executing:
                case JobExecutionState.Persisted:
                    RequestCancel();
                    break;
                case JobExecutionState.Scheduled:
                    MarkCancelled();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// This function is called when a job is already being processed by the scheduler.
        /// In any state, it might be locked
        /// </remarks>
        private void RequestCancel()
        {
            // Force releasing the lock
            this.ReleaseLock(true);

            JobExecutionStatus |= JobExecutionState.CancelRequested;
            Save();
        }

        private void MarkCancelled()
        {
            JobExecutionStatus = JobExecutionState.Cancelled;
            DateFinished = DateTime.Now;

            Save();
        }

        #endregion
        #region Recurring Job Logic Support Functions

        /// <summary>
        /// Reschedules the job instance if it's a recurring job.
        /// </summary>
        /// <returns>The new job instance that is in the queue now.</returns>
        /// <remarks>
        /// The completed job is marked as completed and is stored for reference, so a new
        /// job instance is created with updated properties.
        /// </remarks>
        public JobInstance RescheduleIfRecurring()
        {
            if ((jobExecutionStatus == Registry.JobExecutionState.Completed ||
                jobExecutionStatus == Registry.JobExecutionState.Failed ||
                jobExecutionStatus == Registry.JobExecutionState.Cancelled)
                && scheduleType == ScheduleType.Recurring)
            {
                // Create a copy first
                JobInstance newjob = new JobInstance(this);

                // Reset properties
                newjob.Guid = Guid.Empty;
                newjob.Name = JobInstanceFactory.GenerateRecurringJobID(Context, Name);
                newjob.dateStarted = DateTime.MinValue;
                newjob.dateFinished = DateTime.MinValue;
                newjob.JobExecutionStatus = JobExecutionState.Scheduled;
                newjob.ScheduleTime = GetNextScheduleTime();
                newjob.workflowInstanceId = Guid.Empty;
                newjob.adminRequestTime = DateTime.MinValue;
                newjob.adminRequestData = null;
                newjob.adminRequestResult = -1;
                newjob.exceptionMessage = null;

                // Save new job
                newjob.Save();

                return newjob;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Computes the next time when the recurring job should be executed.
        /// </summary>
        /// <returns>The time of the next execution.</returns>
        /// <remarks>
        /// The calculation is not based on the system time but on the value
        /// of the <see cref="ScheduleTime"/> property.
        /// </remarks>
        public DateTime GetNextScheduleTime()
        {
            DateTime res = DateTime.MinValue;
            DateTime date = scheduleTime.Date;
            TimeSpan time = scheduleTime.TimeOfDay;
            int day = 0;
            int[] mask = null;
            bool found = false;

            // Generate mask arrays
            switch (recurringPeriod)
            {
                case RecurringPeriod.Weekly:
                    day = (int)date.DayOfWeek;
                    mask = GetDaysFromMask(recurringPeriod, recurringMask);
                    break;
                case RecurringPeriod.Monthly:
                    day = date.Day;
                    mask = GetDaysFromMask(recurringPeriod, recurringMask);
                    break;
            }

            // Look up next schedule time in the given interval
            switch (recurringPeriod)
            {
                case RecurringPeriod.Weekly:
                case RecurringPeriod.Monthly:
                    found = false;
                    for (int i = 0; i < mask.Length; i++)
                    {
                        if (mask[i] + 1 > day)
                        {
                            res = date.AddDays(mask[i] + 1 - day);
                            found = true;
                            break;
                        }
                    }
                    break;
            }

            // If no new schedule time in the given interval, skip a period and
            // schedule at the first time that mask allows
            switch (recurringPeriod)
            {
                case RecurringPeriod.Daily:
                    res = date.AddDays(recurringInterval);
                    break;
                case RecurringPeriod.Weekly:
                    if (!found)
                    {
                        res = date.AddDays(7 * recurringInterval + mask[0] - day);
                    }
                    break;
                case RecurringPeriod.Monthly:
                    if (!found)
                    {
                        res = date.AddMonths(recurringInterval);
                        res = new DateTime(res.Year, res.Month, 1);
                        res = res.AddDays(mask[0]);
                    }
                    break;
                default:
                    return DateTime.MinValue;
            }

            return res.Add(time);
        }

        /// <summary>
        /// Converts a bit masked long integer into an array of numbers representing
        /// days of the week or the month
        /// </summary>
        /// <param name="period">Type of the recurring period.</param>
        /// <param name="mask">Bit mask value.</param>
        /// <returns>And array with the days.</returns>
        private int[] GetDaysFromMask(RecurringPeriod period, long mask)
        {
            int max = 0;
            List<int> res = new List<int>();


            switch (period)
            {
                case RecurringPeriod.Weekly:
                    max = 7;
                    break;
                case RecurringPeriod.Monthly:
                    max = 31;
                    break;
            }

            for (int i = 0; i < max; i++)
            {
                if ((mask & 0x00000001) != 0)
                    res.Add(i);

                mask = mask >> 1;
            }

            return res.ToArray();
        }

        /// <summary>
        /// Converts numbers to string representation of the day.
        /// </summary>
        /// <param name="period">Type of the recurring period.</param>
        /// <param name="days">A list of the days.</param>
        /// <returns>An array containing the string representations of days.</returns>
        private string[] GetDayNamesFromMask(RecurringPeriod period, int[] days)
        {
            string[] res = new string[days.Length];
            for (int i = 0; i < days.Length; i++)
            {
                switch (period)
                {
                    case RecurringPeriod.Weekly:
                        res[i] = global::System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.DayNames[i];
                        break;
                    case RecurringPeriod.Monthly:
                        res[i] = days[i].ToString();
                        break;
                }
            }

            return res;
        }

        /// <summary>
        /// Converts numbers to string representation of the day.
        /// </summary>
        /// <returns>An array containing the string representations of days.</returns>
        public string[] GetDayNamesFromMask()
        {
            return GetDayNamesFromMask(recurringPeriod, GetDaysFromMask(recurringPeriod, recurringMask));
        }

        #endregion
    }
}
