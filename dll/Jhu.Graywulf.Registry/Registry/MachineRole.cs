/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Machine Role</b> entity
    /// </summary>
    public partial class MachineRole : Entity
    {
        #region Member Variables

        // --- Background storage for properties ---
        private MachineRoleType machineRoleType;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Gets or sets the type of this <b>Machine Role</b>, like <i>Stand alone server, Sliced group, Mirrored group</i> etc.
        /// </summary>
        [DBColumn]
        public MachineRoleType MachineRoleType
        {
            get { return machineRoleType; }
            set { machineRoleType = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Cluster</b> object to which this <b>Machine Role</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        [XmlIgnore]
        public Cluster Cluster
        {
            get { return (Cluster)ParentReference.Value; }
        }

        [XmlIgnore]
        public Dictionary<string, Machine> Machines
        {
            get { return GetChildren<Machine>(); }
            set { SetChildren<Machine>(value); }
        }

        [XmlIgnore]
        public Dictionary<string, ServerVersion> ServerVersions
        {
            get { return GetChildren<ServerVersion>(); }
            set { SetChildren<ServerVersion>(value); }
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
        public MachineRole()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Machine Role</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public MachineRole(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public MachineRole(Cluster parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Machine Role</b> objects.
        /// </summary>
        /// <param name="old">The <b>Machine Role</b> to copy from.</param>
        public MachineRole(MachineRole old)
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
            base.EntityType = EntityType.MachineRole;
            base.EntityGroup = EntityGroup.Cluster | EntityGroup.Jobs;

            this.machineRoleType = MachineRoleType.Unknown;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Machine Role</b> object to create the deep copy from.</param>
        private void CopyMembers(MachineRole old)
        {
            this.machineRoleType = old.machineRoleType;
        }

        public override object Clone()
        {
            return new MachineRole(this);
        }

        protected override Type[] CreateChildTypes()
        {
            return new Type[]
            {
                typeof(Machine),
                typeof(ServerVersion),
            };
        }

        #endregion
    }
}
