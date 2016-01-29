/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Machine</b> entity
    /// </summary>
    public partial class Machine : Entity
    {
        #region Member Variables

        // --- Background storage for properties ---
        private ExpressionProperty hostName;
        private ExpressionProperty adminUrl;
        private ExpressionProperty deployUncPath;

        #endregion
        #region Member Access Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.Machine; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Cluster; }
        }

        /// <summary>
        /// Gets or sets NetBIOS name of the server.
        /// </summary>
        [DBColumn(Size = 50)]
        public ExpressionProperty HostName
        {
            get { return hostName; }
            set { hostName = value; }
        }

        /// <summary>
        /// Gets or sets URL to the servers administrative web application
        /// </summary>
        [DBColumn(Size = 1024)]
        public ExpressionProperty AdminUrl
        {
            get { return adminUrl; }
            set { adminUrl = value; }
        }

        /// <summary>
        /// Gets or sets the UNC path to the share on the server for installing <b>Deployment Packages</b>.
        /// </summary>
        [DBColumn(Size = 1024)]
        public ExpressionProperty DeployUncPath
        {
            get { return deployUncPath; }
            set { deployUncPath = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Machine Role</b> object to which this <b>Machine</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        [XmlIgnore]
        public MachineRole MachineRole
        {
            get { return (MachineRole)ParentReference.Value; }
        }

        [XmlIgnore]
        public Dictionary<string, DiskGroup> DiskGroups
        {
            get { return GetChildren<DiskGroup>(); }
            set { SetChildren<DiskGroup>(value); }
        }

        [XmlIgnore]
        public Dictionary<string, ServerInstance> ServerInstances
        {
            get { return GetChildren<ServerInstance>(); }
            set { SetChildren<ServerInstance>(value); }
        }

        [XmlIgnore]
        public Dictionary<string, QueueInstance> QueueInstances
        {
            get { return GetChildren<QueueInstance>(); }
            set { SetChildren<QueueInstance>(value); }
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
        public Machine()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Machine</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public Machine(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public Machine(MachineRole parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Machine</b> objects.
        /// </summary>
        /// <param name="old">The <b>Machine</b> to copy from.</param>
        public Machine(Machine old)
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
            this.hostName = new ExpressionProperty(this, Constants.MachineHostName);
            this.adminUrl = new ExpressionProperty(this, Constants.MachineAdminUrl);
            this.deployUncPath = new ExpressionProperty(this, Constants.MachineDeployUncPath);
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Machine</b> object to create the deep copy from.</param>
        private void CopyMembers(Machine old)
        {
            this.hostName = old.hostName;
            this.adminUrl = new ExpressionProperty(old.adminUrl);
            this.deployUncPath = new ExpressionProperty(old.deployUncPath);
        }

        public override object Clone()
        {
            return new Machine(this);
        }

        protected override EntityType[] CreateChildTypes()
        {
            return new EntityType[]
            {
                EntityType.DiskGroup,
                EntityType.ServerInstance,
                EntityType.QueueInstance,
            };
        }

        #endregion
    }
}
