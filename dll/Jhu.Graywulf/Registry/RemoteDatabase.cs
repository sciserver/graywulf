/* Copyright */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Implements the functionality related to a database server cluster's <b>Remote Database</b> entity
    /// </summary>
    public partial class RemoteDatabase : Entity
    {
        #region Member Variables

        // --- Background storage for properties ---
        private string providerName;
        private string connectionString;
        private bool integratedSecurity;
        private string username;
        private string password;
        private bool requiresSshTunnel;
        private string sshHostName;
        private int sshPortNumber;
        private string sshUsername;
        private string sshPassword;

        #endregion
        #region Member Access Properties

        [DBColumn(Size = 128)]
        public string ProviderName
        {
            get { return providerName; }
            set { providerName = value; }
        }

        [DBColumn(Size = 1024)]
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        [DBColumn]
        public bool IntegratedSecurity
        {
            get { return integratedSecurity; }
            set { integratedSecurity = value; }
        }

        [DBColumn(Size = 50)]
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        [DBColumn(Size = 50)]
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        [DBColumn]
        public bool RequiresSshTunnel
        {
            get { return requiresSshTunnel; }
            set { requiresSshTunnel = value; }
        }

        [DBColumn(Size = 80)]
        public string SshHostName
        {
            get { return sshHostName; }
            set { sshHostName = value; }
        }

        [DBColumn]
        public int SshPortNumber
        {
            get { return sshPortNumber; }
            set { sshPortNumber = value; }
        }

        [DBColumn(Size = 50)]
        public string SshUsername
        {
            get { return sshUsername; }
            set { sshUsername = value; }
        }

        [DBColumn(Size = 50)]
        public string SshPassword
        {
            get { return sshPassword; }
            set { sshPassword = value; }
        }

        #endregion
        #region Navigation Properties

        /// <summary>
        /// Gets the <b>Federation</b> object to which this <b>Remote Database</b> belongs.
        /// </summary>
        /// <remarks>
        /// This property does do lazy loading, no calling of a loader function is necessary, but
        /// a valid object context with an open database connection must be set.
        /// </remarks>
        public Federation Federation
        {
            get { return (Federation)ParentReference.Value; }
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
        public RemoteDatabase()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new <b>Remote Database</b> object and setting object context.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        public RemoteDatabase(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Constructor for creating a new entity with object context and parent entity set.
        /// </summary>
        /// <param name="context">An object context class containing session information.</param>
        /// <param name="parent">The parent entity in the entity hierarchy.</param>
        public RemoteDatabase(Context context, Federation parent)
            : base(context, parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy contructor for doing deep copy of the <b>Remote Database</b> objects.
        /// </summary>
        /// <param name="old">The <b>Remote Database</b> to copy from.</param>
        public RemoteDatabase(RemoteDatabase old)
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
            base.EntityType = EntityType.RemoteDatabase;
            base.EntityGroup = EntityGroup.Federation;

            this.providerName = String.Empty;
            this.connectionString = String.Empty;
            this.integratedSecurity = false;
            this.username = String.Empty;
            this.password = String.Empty;
            this.requiresSshTunnel = false;
            this.sshHostName = String.Empty;
            this.sshPortNumber = 22;
            this.sshUsername = String.Empty;
            this.sshPassword = String.Empty;
        }

        /// <summary>
        /// Creates a deep copy of the passed object.
        /// </summary>
        /// <param name="old">A <b>Database Definition</b> object to create the deep copy from.</param>
        private void CopyMembers(RemoteDatabase old)
        {
            this.providerName = old.providerName;
            this.connectionString = old.connectionString;
            this.integratedSecurity = old.integratedSecurity;
            this.username = old.username;
            this.password = old.password;
            this.requiresSshTunnel = old.requiresSshTunnel;
            this.sshHostName = old.sshHostName;
            this.sshPortNumber = old.sshPortNumber;
            this.sshUsername = old.sshUsername;
            this.sshPassword = old.sshPassword;
        }

        #endregion
    }
}
