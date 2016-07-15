/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Database Definition</b> entity
    /// </summary>
    public partial class QueueInstance : Entity
    {
        public enum ReferenceType : int
        {
            QueueDefinition = 1,
        }

        #region Member Variables

        // --- Background storage for properties ---
        private int maxOutstandingJobs;
        private int timeout;

        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.QueueInstance; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Cluster; }
        }

        /// <summary>
        /// Gets or sets the maximum number of concurrently executing jobs.
        /// </summary>
        [DBColumn]
        public int MaxOutstandingJobs
        {
            get { return maxOutstandingJobs; }
            set { maxOutstandingJobs = value; }
        }

        /// <summary>
        /// Maximum number of seconds for the job to run.
        /// </summary>
        /// <remarks>
        /// Zero value means no timeout.
        /// </remarks>
        [DBColumn]
        [DefaultValue(0)]
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Machine</b> object to which the <b>Queue Instance</b> belongs to.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        public Machine Machine
        {
            get { return (Machine)ParentReference.Value; }
        }

        /// <summary>
        /// Gets the reference object to the queue definition associated with this queue instance.
        /// </summary>
        [XmlIgnore]
        public EntityReference<QueueDefinition> QueueDefinitionReference
        {
            get { return (EntityReference<QueueDefinition>)EntityReferences[(int)ReferenceType.QueueDefinition]; }
        }

        /// <summary>
        /// Gets the queue definition associated with this queue instance.
        /// </summary>
        [XmlIgnore]
        public QueueDefinition QueueDefinition
        {
            get { return QueueDefinitionReference.Value; }
            set { QueueDefinitionReference.Value = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("QueueDefinition")]
        public string QueueDefinition_ForXml
        {
            get { return QueueDefinitionReference.Name; }
            set { QueueDefinitionReference.Name = value; }
        }

        [XmlIgnore]
        public Dictionary<string, JobInstance> JobInstances
        {
            get { return GetChildren<JobInstance>(); }
            set { SetChildren<JobInstance>(value); }
        }

        #endregion
        #region Validation Properties
        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public QueueInstance()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Database Definition</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public QueueInstance(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public QueueInstance(Machine parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Database Definition</b> objects.
        /// </summary>
        /// <param name="old">The <b>Database Definition</b> to copy from.</param>
        public QueueInstance(QueueInstance old)
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
        private void InitializeMembers()
        {
            this.RunningState = RunningState.Paused;
            this.maxOutstandingJobs = 1;
            this.timeout = 0;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Database Definition</b> object to create the deep copy from.</param>
        private void CopyMembers(QueueInstance old)
        {
            this.maxOutstandingJobs = old.maxOutstandingJobs;
            this.timeout = 0;
        }

        public override object Clone()
        {
            return new QueueInstance(this);
        }

        protected override IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[]
            {
                new EntityReference<QueueDefinition>((int)ReferenceType.QueueDefinition),
            };
        }

        protected override EntityType[] CreateChildTypes()
        {
            return new EntityType[]
            {
                EntityType.JobInstance,
            };
        }

        #endregion
    }
}
