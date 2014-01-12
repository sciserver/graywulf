/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// The class represents a checkpoint for the <see cref="JobDefinition"/> and <see cref="JobInstance"/>
    /// classes.
    /// </summary>
    /// <remarks>
    /// Checkpoints extracted automatically from workflow derived from the <b>JobWorkflow</b> class.
    /// This class is only used by the Graywulf framework, checkpoints should not be added manually.
    /// Database access functions are implemented in the <see cref="JobDefinition"/> and <see cref="JobInstance"/>
    /// classes.
    /// </remarks>
    public class JobCheckpoint
    {
        #region Member Variables

        private int sequenceNumber;
        private string name;
        private JobExecutionState executionStatus;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Gets or sets the ordinal number of the checkpoint in the sequence.
        /// </summary>
        [XmlAttribute]
        public int SequenceNumber
        {
            get { return sequenceNumber; }
            set { sequenceNumber = value; }
        }

        /// <summary>
        /// Gets or sets the name of the checkpoint.
        /// </summary>
        [XmlAttribute]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the execution status of the checkpoint.
        /// </summary>
        /// <remarks>
        /// This property is only meaningful for checkpoints of <b>JobInstance</b> classes.
        /// The status of checkpoints is set automatically by the <b>CheckpointActivity</b> activities.
        /// </remarks>
        [XmlIgnore]
        public JobExecutionState ExecutionStatus
        {
            get { return executionStatus; }
            set { executionStatus = value; }
        }

        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor which initializes private members.
        /// </summary>
        public JobCheckpoint()
        {
            InitializeMembers();
        }

        #endregion
        #region Initializer Functions

        /// <summary>
        /// Initializes private members to their default values.
        /// </summary>
        private void InitializeMembers()
        {
            this.sequenceNumber = -1;
            this.name = string.Empty;
            this.executionStatus = JobExecutionState.Unknown;
        }

        #endregion
    }
}
