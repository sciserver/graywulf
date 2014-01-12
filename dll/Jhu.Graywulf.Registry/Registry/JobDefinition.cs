/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;
using System.Configuration;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Database Definition</b> entity
    /// </summary>
    public partial class JobDefinition : Entity
    {
        #region Member Variables

        // --- Background storage for properties ---
        private string workflowTypeName;

        private Dictionary<string, JobParameter> parameters;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Gets or sets the type name of the workflow class with namespace information.
        /// </summary>
        [DBColumn(Size = 1024)]
        public string WorkflowTypeName
        {
            get { return workflowTypeName; }
            set { workflowTypeName = value; }
        }

        /// <summary>
        /// Gets a dictionary with the workflow input parameters.
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, JobParameter> Parameters
        {
            get { return parameters; }
        }

        [XmlArray("Parameters")]
        public JobParameter[] Parameters_ForXml
        {
            get { return parameters.Values.ToArray(); }
            set
            {
                parameters = new Dictionary<string, JobParameter>();
                foreach (var p in value)
                {
                    parameters.Add(p.Name, p);
                }
            }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Federation</b> object to which this <b>Database Definition</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        [XmlIgnore]
        public Federation Federation
        {
            get { return (Federation)ParentReference.Value; }
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
        public JobDefinition()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Database Definition</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public JobDefinition(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public JobDefinition(Cluster parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public JobDefinition(Federation parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Database Definition</b> objects.
        /// </summary>
        /// <param name="old">The <b>Database Definition</b> to copy from.</param>
        public JobDefinition(JobDefinition old)
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
            base.EntityType = EntityType.JobDefinition;
            base.EntityGroup = EntityGroup.Jobs;

            this.workflowTypeName = string.Empty;

            this.parameters = new Dictionary<string, JobParameter>();
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Database Definition</b> object to create the deep copy from.</param>
        private void CopyMembers(JobDefinition old)
        {
            this.parameters = new Dictionary<string, JobParameter>(old.parameters);
        }

        public override object Clone()
        {
            return new JobDefinition(this);
        }

        #endregion
        #region Workflow Support Functions

        /// <summary>
        /// Queries the workflow class definition for input parameters that 
        /// should be handled by the scheduler system.
        /// </summary>
        /// <remarks>
        /// The function loads the workflow assembly and queries its interface for 
        /// parameters flagged as input parameters. The logic is implemented in
        /// the <see cref="Jhu.Graywulf.Activities.ReflectionHelper"/> class.
        /// Workflow input parameters should be flagged with the WorkflowParameterAttribute attribute.
        /// </remarks>
        public void DiscoverWorkflowParameters()
        {
            parameters.Clear();

            var rh = JobReflectionHelper.CreateInstance(this.workflowTypeName);
            foreach (var par in rh.GetParameters().Values)
            {
                parameters.Add(par.Name, new JobParameter(par));
            }
        }

        public JobInstance CreateJobInstance(string queueName, ScheduleType scheduleType)
        {
            JobInstance job = new JobInstance(Context);

            job.Name = GenerateJobID();
            job.JobDefinitionReference.Name = GetFullyQualifiedName();
            job.ParentReference.Name = queueName;
            job.WorkflowTypeName = this.workflowTypeName;
            job.JobExecutionStatus = Jhu.Graywulf.Registry.JobExecutionState.Scheduled;
            job.ScheduleType = scheduleType;

            // Create workflow parameters
            var rh = JobReflectionHelper.CreateInstance(this.workflowTypeName);
            foreach (var par in rh.GetParameters().Values)
            {
                job.Parameters.Add(par.Name, new JobParameter(par));
            }

            return job;
        }

        public string GenerateJobID()
        {
            return String.Format("{0}_{1:yyMMddHHmmssff}", Context.UserName, DateTime.Now);
        }

        #endregion
    }
}
