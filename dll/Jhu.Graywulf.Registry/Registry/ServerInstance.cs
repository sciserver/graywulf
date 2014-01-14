/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.ComponentModel;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Server Instance</b> entity
    /// </summary>
    public partial class ServerInstance : Entity
    {
        public enum ReferenceType : int
        {
            ServerVersion = 1,
        }

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
            get { return EntityType.ServerInstance; }
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

        [XmlIgnore]
        public ServerVersion ServerVersion
        {
            get { return ServerVersionReference.Value; }
            set { ServerVersionReference.Value = value; }
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
        /// Gets the <b>Machine</b> object to which the <b>Server Instance</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        public Machine Machine
        {
            get { return (Machine)ParentReference.Value; }
        }

        [XmlIgnore]
        public EntityReference<ServerVersion> ServerVersionReference
        {
            get { return (EntityReference<ServerVersion>)EntityReferences[(int)ReferenceType.ServerVersion]; }
        }

        [XmlElement("ServerVersion")]
        public string ServerVersion_ForXml
        {
            get { return ServerVersionReference.Name; }
            set { ServerVersionReference.Name = value; }
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
        public ServerInstance()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Server Instance</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public ServerInstance(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public ServerInstance(Machine parent)
            : base(parent.Context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Server Instance</b> objects.
        /// </summary>
        /// <param name="old">The <b>Server Instance</b> to copy from.</param>
        public ServerInstance(ServerInstance old)
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
        private void CopyMembers(ServerInstance old)
        {
            this.instanceName = old.instanceName;
            this.integratedSecurity = old.integratedSecurity;
            this.adminUser = old.adminUser;
            this.adminPassword = old.adminPassword;
        }

        public override object Clone()
        {
            return new ServerInstance(this);
        }

        protected override IEntityReference[] CreateEntityReferences()
        {
            return new IEntityReference[]
            {
                new EntityReference<ServerVersion>((int)ReferenceType.ServerVersion),
            };
        }

        #endregion

        /// <summary>
        /// Returns the full name of the server instance.
        /// </summary>
        /// <returns>The full name of the server instance.</returns>
        /// <remarks>
        /// If the <see cref="InstanceName"/> property is empty only the
        /// machine name is returned, otherwise the two are combined with
        /// a backslash.
        /// </remarks>
        public string GetCompositeName()
        {
            // Build string from server name and instance name
            string name = this.Machine.HostName.ResolvedValue;
            if (this.InstanceName.Trim() != string.Empty)
                name += "\\" + this.InstanceName;

            return name;
        }

        /// <summary>
        /// Returns a SqlConnectionStringBuilder object initialized
        /// to point to this server instance.
        /// </summary>
        /// <returns>The connection string builder objects.</returns>
        public SqlConnectionStringBuilder GetConnectionString()
        {
            SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder();
            csb.DataSource = Machine.HostName.ResolvedValue;
            if (this.instanceName != string.Empty)
                csb.DataSource += @"\" + this.instanceName;

            csb.Enlist = false;
            csb.ConnectTimeout = 30;    // *** TODO: take from config

            if (this.integratedSecurity)
            {
                csb.IntegratedSecurity = true;
            }
            else
            {
                csb.IntegratedSecurity = false;
                csb.UserID = this.adminUser;
                csb.Password = this.adminPassword;
            }

            return csb;
        }

        public Schema.SqlServer.SqlServerDataset GetDataset()
        {
            return new Schema.SqlServer.SqlServerDataset()
            {
                ConnectionString = GetConnectionString().ConnectionString
            };
        }
    }
}
