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
        /// Singleton object of <see cref="ContextManager"/> to be used for creating new <see cref="RegistryContext"/> objects.
        /// </summary>
        public static readonly ContextManager Instance = new ContextManager();  // Singleton

        #endregion
        #region Member Variables

        private string connectionString;
        private string smtpString;

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
            var config = Configuration;

            if (config != null)
            {
                this.connectionString = config.ConnectionString;
                this.smtpString = string.Empty;

                this.clusterName = config.ClusterName;
                this.domainName = config.DomainName;
                this.federationName = config.FederationName;
            }
        }

        #endregion
        #region Context Creation Functions

        public RegistryContext CreateContext()
        {
            return CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit);
        }

        public RegistryContext CreateContext(ConnectionMode connectionMode, TransactionMode transactionMode)
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
        public RegistryContext CreateContext(string connectionString, ConnectionMode connectionMode, TransactionMode transactionMode)
        {
            var context = new RegistryContext(Logging.LoggingContext.Current)
            {
                ConnectionString = connectionString,
                ConnectionMode = connectionMode,
                TransactionMode = transactionMode,
            };

            context.ClusterReference.Name = clusterName;
            context.DomainReference.Name = domainName;
            context.FederationReference.Name = federationName;

            return context;
        }

        // TODO: delete
        /*
        public RegistryContext CreateContext(IJobActivity activity, CodeActivityContext activityContext, ConnectionMode connectionMode, TransactionMode transactionMode)
        {
            /*var context = new RegistryContext(activity, activityContext)
            {
                ConnectionString = connectionString,
                ConnectionMode = connectionMode,
                TransactionMode = transactionMode,
            };

            context.EnsureEntitiesLoaded();

            return context;
            

            return null;
        }
    */

        #endregion
    }
}
