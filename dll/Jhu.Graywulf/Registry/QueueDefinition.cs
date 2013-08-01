/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Database Definition</b> entity
    /// </summary>
    public partial class QueueDefinition : Entity
    {
        #region Member Variables

        // --- Background storage for properties ---
        private int maxOutstandingJobs;
        private int timeout;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Gets or sets the default maximum number of concurrently executing jobs.
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
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Cluster</b> object to which this <b>Database Definition</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// Null if the <b>Queue Definition</b> is not a direct child of a <b>Cluster</b>.
        /// </remarks>
        public Cluster Cluster
        {
            get { return ParentReference.Value as Cluster; }
        }

        /// <summary>
        /// Gets the <b>Federation</b> object to which this <b>Database Definition</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// Null if the <b>Queue Definition</b> is not a direct child of a <b>Federation</b>.
        /// </remarks>
        public Federation Federation
        {
            get { return ParentReference.Value as Federation; }
        }

        #endregion
        #region Validation Properties
        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// The default constructor is required for XML and binary serialization. Do not use this.
        /// </remarks>
        public QueueDefinition()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Database Definition</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public QueueDefinition(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public QueueDefinition(Context context, Cluster parent)
            : base(context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public QueueDefinition(Context context, Federation parent)
            : base(context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Database Definition</b> objects.
        /// </summary>
        /// <param name="old">The <b>Database Definition</b> to copy from.</param>
        public QueueDefinition(QueueDefinition old)
            : base(old)
        {
            CopyMembers(old);
        }

        #endregion
        #region Initializer Functions

        /// <summary>
        /// Initializes member variables to their initial values.
        /// </summary>
        /// <remarks>
        /// This function is called by the contructors.
        /// </remarks>
        private void InitializeMembers()
        {
            base.EntityType = EntityType.QueueDefinition;
            base.EntityGroup = EntityGroup.Jobs;

            this.maxOutstandingJobs = 1;
            this.timeout = 0;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Database Definition</b> object to create the deep copy from.</param>
        private void CopyMembers(QueueDefinition old)
        {
            this.maxOutstandingJobs = old.maxOutstandingJobs;
            this.timeout = old.timeout;
        }

        #endregion
    }
}
