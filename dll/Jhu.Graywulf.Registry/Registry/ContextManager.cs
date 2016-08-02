using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Activities;
using System.Configuration;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// Manages context creation
    /// </summary>
    /// <remarks>
    /// Do not create instances of this class directly. Access a
    /// singleton instance of the class through the <see cref="Instance"/> property.
    /// </remarks>
    public class ContextManager
    {
        #region Static members
        public static RegistryConfiguration Configuration
        {
            get
            {
                return (RegistryConfiguration)ConfigurationManager.GetSection("jhu.graywulf/registry");
            }
        }

        /// <summary>
        /// Singleton object of <see cref="ContextManager"/> to be used for creating new <see cref="Context"/> objects.
        /// </summary>
        public static readonly ContextManager Instance = new ContextManager();  // Singleton

        #endregion
        #region Member Variables

        private object syncRoot = new object();

        private string connectionString;
        private string smtpString;

        private Guid userGuid;
        private string userName;

        private string clusterName;
        private string domainName;
        private string federationName;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Gets or sets the database connection string.
        /// </summary>
        public string ConnectionString
        {
            get { return this.connectionString; }
            set { this.connectionString = value; }
        }

        /// <summary>
        /// Gets or sets the SMTP server connection string.
        /// </summary>
        public string SmtpString
        {
            get { return this.smtpString; }
            set { this.smtpString = value; }
        }

        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor that initializes private members to their
        /// default values.
        /// </summary>
        public ContextManager()
        {
            InitializeMembers();
        }

        #endregion
        #region Initializer Functions

        /// <summary>
        /// Initializes private members to their default values
        /// </summary>
        private void InitializeMembers()
        {
            this.connectionString = Configuration.ConnectionString;
            this.smtpString = string.Empty;

            this.userGuid = Guid.Empty;
            this.userName = null;

            this.clusterName = Configuration.ClusterName;
            this.domainName = Configuration.DomainName;
            this.federationName = Configuration.FederationName;
        }

        #endregion
        #region Context Creation Functions

        public Context CreateContext(ConnectionMode connectionMode, TransactionMode transactionMode)
        {
            return CreateContext(this.connectionString, connectionMode, transactionMode);
        }

        /// <summary>
        /// Creates a context with an optionally open database and SMTP connection.
        /// </summary>
        /// <param name="openConnection">True if a database connection should be opened.</param>
        /// <param name="beginTransaction">True if a transaction is required.</param>
        /// <param name="openSmtp">True if an SMTP connection should be opened.</param>
        /// <returns>A valid connection.</returns>
        public Context CreateContext(string connectionString, ConnectionMode connectionMode, TransactionMode transactionMode)
        {
            var context = new Context()
            {
                ConnectionString = connectionString,
                ConnectionMode = connectionMode,
                TransactionMode = transactionMode,

                UserGuid = userGuid,
                UserName = userName,
            };

            context.ClusterReference.Name = clusterName;
            context.DomainReference.Name = domainName;
            context.FederationReference.Name = federationName;

            return context;
        }

        public Context CreateContext(IGraywulfActivity activity, CodeActivityContext activityContext, ConnectionMode connectionMode, TransactionMode transactionMode)
        {
            var context = new Context(activity, activityContext)
            {
                ConnectionString = connectionString,
                ConnectionMode = connectionMode,
                TransactionMode = transactionMode,
            };

            return context;
        }

        #endregion
    }
}
