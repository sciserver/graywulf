/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Server Versopm</b> entity
    /// </summary>
    public partial class ServerVersion : Entity
    {
        #region Member Variables

        // --- Background storage for properties ---
        private string instanceName;
        private bool integratedSecurity;
        private string adminUser;
        private string adminPassword;

        #endregion
        #region Properties

        [XmlIgnore]
        public override EntityType EntityType
        {
            get { return EntityType.ServerVersion; }
        }

        [XmlIgnore]
        public override EntityGroup EntityGroup
        {
            get { return EntityGroup.Cluster; }
        }

        /// <summary>
        /// Gets or sets the name of the SQL Server instance
        /// </summary>
        [DBColumn(Size = 50)]
        [DefaultValue("")]
        public string InstanceName
        {
            get { return instanceName; }
            set { instanceName = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether the <b>Server Instance</b> is to be accessed using integrated
        /// windows authentication.
        /// </summary>
        /// <remarks>
        /// If the value is set to <b>false</b> the administrator username and password must be supplied
        /// in the <see cref="AdminUser"/> and the <see cref="AdminPassword"/> properties.
        /// </remarks>
        [DBColumn]
        [DefaultValue(true)]
        public bool IntegratedSecurity
        {
            get { return integratedSecurity; }
            set { integratedSecurity = value; }
        }

        /// <summary>
        /// Gets or sets the username of the SQL Server user account for configuring the <b>Server Instance</b>
        /// </summary>
        [DBColumn(Size = 50)]
        [DefaultValue("")]
        public string AdminUser
        {
            get { return adminUser; }
            set { adminUser = value; }
        }

        /// <summary>
        /// Gets or sets the password of the SQL Server user account for configuring the <b>Server Instance</b>
        /// </summary>
        [DBColumn(Size = 50)]
        [DefaultValue("")]
        public string AdminPassword
        {
            get { return adminPassword; }
            set { adminPassword = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Machine Role</b> object to which the <b>Server Version</b> belongs.
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
        public ServerVersion()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Server Version</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public ServerVersion(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public ServerVersion(MachineRole parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Server Version</b> objects.
        /// </summary>
        /// <param name="old">The <b>Server Version</b> to copy from.</param>
        public ServerVersion(ServerVersion old)
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
            this.instanceName = string.Empty;
            this.integratedSecurity = true;
            this.adminUser = string.Empty;
            this.adminPassword = string.Empty;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Server Instance</b> object to create the deep copy from.</param>
        private void CopyMembers(ServerVersion old)
        {
            this.instanceName = old.instanceName;
            this.integratedSecurity = old.integratedSecurity;
            this.adminUser = old.adminUser;
            this.adminPassword = old.adminPassword;
        }

        internal override bool CompareMembers(Entity other)
        {
            bool eq = base.CompareMembers(other);
            var o = other as ServerVersion;

            eq &= this.instanceName == o.instanceName;
            eq &= this.integratedSecurity == o.integratedSecurity;
            eq &= this.adminUser == o.adminUser;
            eq &= this.adminPassword == o.adminPassword;

            return eq;
        }

        internal override void UpdateMembers(Entity other)
        {
            base.UpdateMembers(other);
            var o = other as ServerVersion;
            CopyMembers(o);
        }

        public override object Clone()
        {
            return new ServerVersion(this);
        }

        #endregion

    }
}
