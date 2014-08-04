/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Represents a dependency between a job instance and another
    /// that is supposed to complete before the current job.
    /// </summary>
    public partial class JobInstanceDependency : Entity
    {
        public enum ReferenceType : int
        {
            PredecessorJobInstance = 1
        }

        #region Member Variables

        private JobDependencyType dependencyType;

        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.JobInstanceDependency; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Jobs; }
        }

        [DBColumn]
        public JobDependencyType DependencyType
        {
            get { return dependencyType; }
            set { dependencyType = value; }
        }

        #endregion
        #region Navigation Properties

        [XmlIgnore]
        public JobInstance JobInstance
        {
            get
            {
                return (JobInstance)ParentReference.Value;
            }
        }

        [XmlIgnore]
        public EntityReference<JobInstance> PredecessorJobInstanceReference
        {
            get { return (EntityReference<JobInstance>)EntityReferences[(int)ReferenceType.PredecessorJobInstance]; }
        }

        [XmlIgnore]
        public JobInstance PredecessorJobInstance
        {
            get { return PredecessorJobInstanceReference.Value; }
            set { PredecessorJobInstanceReference.Value = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        [XmlElement("PredecessorJobInstance")]
        public string PredecessorJobInstance_ForXml
        {
            get { return PredecessorJobInstanceReference.Name; }
            set { PredecessorJobInstanceReference.Name = value; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public JobInstanceDependency()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Constructor for creating a new object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public JobInstanceDependency(Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public JobInstanceDependency(JobInstance parent)
            : base(parent.Context, parent)
        {
            InitializeMembers(new StreamingContext());
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the objects.
        /// </summary>
        /// <param name="old">The object to copy from.</param>
        public JobInstanceDependency(JobInstanceDependency old)
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
            this.dependencyType = JobDependencyType.Unknown;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Slice</b> object to create the deep copy from.</param>
        private void CopyMembers(JobInstanceDependency old)
        {
            this.dependencyType = old.dependencyType;
        }

        public override object Clone()
        {
            return new JobInstanceDependency(this);
        }

        protected override IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[]
            {
                new EntityReference<JobInstance>((int)ReferenceType.PredecessorJobInstance),
            };
        }

        #endregion
    }
}
