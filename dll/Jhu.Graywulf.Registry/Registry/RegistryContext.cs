using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Activities;
using Jhu.Graywulf.Logging;
using Jhu.Graywulf.Activities;

namespace Jhu.Graywulf.Registry
{

    /// <summary>
    /// Represents an object context that is used for managing database connection and transaction,
    /// SMTP server connection and workflow activity context.
    /// </summary>
    public class RegistryContext : IRegistryContextObject, IDisposable
    {
        #region Member Variables

#if DEBUG
        private int sqlSpId;
#endif

        private bool isValid;

        // TODO: move these to entity search
        private bool showHidden;
        private bool showDeleted;
        //

        private string connectionString;
        private ConnectionMode connectionMode;
        private TransactionMode transactionMode;

        private SqlConnection databaseConnection;
        private SqlTransaction databaseTransaction;

        private EntityCache entityCache;

        private EntityReference<Cluster> clusterReference;
        private EntityReference<Domain> domainReference;
        private EntityReference<Federation> federationReference;
        private EntityReference<User> userReference;
        private EntityReference<JobInstance> jobReference;

        #endregion
        #region Member Access Properties

        public bool IsValid
        {
            get { return isValid; }
        }
        
        /// <summary>
        /// Gets or sets the value determining whether search functions will return entities
        /// flagged as hidden.
        /// </summary>
        public bool ShowHidden
        {
            get { return showHidden; }
            set { showHidden = value; }
        }

        /// <summary>
        /// Gets or sets the value determining whether search functions will return entities
        /// flagged as deleted.
        /// </summary>
        public bool ShowDeleted
        {
            get { return showDeleted; }
            set { showDeleted = value; }
        }

        public string ConnectionString
        {
            get { return connectionString; }
            internal set { connectionString = value; }
        }

        public ConnectionMode ConnectionMode
        {
            get { return connectionMode; }
            internal set { connectionMode = value; }
        }

        public TransactionMode TransactionMode
        {
            get { return transactionMode; }
            set { transactionMode = value; }
        }

        RegistryContext IRegistryContextObject.RegistryContext
        {
            get { return this; }
            set { throw new InvalidOperationException(); }
        }

        internal EntityCache EntityCache
        {
            get { return entityCache; }
        }

        /// <summary>
        /// Gets the database connection associated with this context.
        /// </summary>
        public SqlConnection DatabaseConnection
        {
            get
            {
                EnsureOpenConnection();
                return this.databaseConnection;
            }
            internal set { this.databaseConnection = value; }
        }

        /// <summary>
        /// Gets the database transaction associated with this context.
        /// </summary>
        public SqlTransaction DatabaseTransaction
        {
            get
            {
                EnsureOpenTransaction();
                return this.databaseTransaction;
            }
            internal set { this.databaseTransaction = value; }
        }

        public EntityReference<Cluster> ClusterReference
        {
            get { return clusterReference; }
        }

        public Cluster Cluster
        {
            get { return clusterReference.Value; }
        }

        public EntityReference<Domain> DomainReference
        {
            get { return domainReference; }
        }

        public Domain Domain
        {
            get { return domainReference.Value; }
        }

        public EntityReference<Federation> FederationReference
        {
            get { return federationReference; }
        }

        public Federation Federation
        {
            get { return federationReference.Value; }
        }

        public EntityReference<User> UserReference
        {
            get { return userReference; }
        }

        public User User
        {
            get { return userReference.Value;  }
        }

        public EntityReference<JobInstance> JobReference
        {
            get { return jobReference; }
        }

        /// <summary>
        /// Gets or sets the guid of the job which creates the context.
        /// </summary>
        public JobInstance Job
        {
            get { return jobReference.Value; }
        }

        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor that creates a context object and
        /// initializes private members to their default values.
        /// </summary>
        internal RegistryContext(LoggingContext outerContext)
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
            this.isValid = true;

            this.showHidden = false;
            this.showDeleted = false;

            this.connectionString = null;
            this.connectionMode = ConnectionMode.None;
            this.transactionMode = TransactionMode.None;

            this.databaseConnection = null;
            this.databaseTransaction = null;

            this.entityCache = new EntityCache();

            this.clusterReference = new EntityReference<Cluster>(this);
            this.domainReference = new EntityReference<Domain>(this);
            this.federationReference = new EntityReference<Federation>(this);
            this.userReference = new EntityReference<User>(this);
            this.jobReference = new EntityReference<JobInstance>(this);

            InitializeFromOuterContext();
        }

        internal void InitializeFromOuterContext()
        {
            var identity = System.Threading.Thread.CurrentPrincipal?.Identity as AccessControl.GraywulfIdentity;

            if (identity != null)
            {
                this.userReference.Guid = identity.UserReference.Guid;
            }

            var jobContext = JobContext.Current;

            if (jobContext != null)
            {
                this.jobReference.Guid = jobContext.JobGuid;
            }
        }

        /// <summary>
        /// Disposes the context and commits the SQL transaction, closes the connection.
        /// </summary>
        public void Dispose()
        {
            if (databaseTransaction != null)
            {
                switch (transactionMode)
                {
                    case TransactionMode.None:
                        break;
                    case TransactionMode.AutoCommit:
                        CommitTransaction();
                        break;
                    case TransactionMode.DirtyRead:
                    case TransactionMode.ManualCommit:
                        RollbackTransaction();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (databaseConnection != null)
            {
                CloseConnection();
            }

            this.entityCache.Dispose();
            this.entityCache = null;

            isValid = false;
        }

        #endregion

        private void EnsureOpenConnection()
        {
            if (!isValid)
            {
                throw new InvalidOperationException();
            }

            if (databaseConnection == null)
            {
                switch (connectionMode)
                {
                    case ConnectionMode.None:
                        throw new InvalidOperationException();
                    case ConnectionMode.AutoOpen:
                        OpenConnection();
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void OpenConnection()
        {
            databaseConnection = new SqlConnection(connectionString);
            databaseConnection.Open();

#if DEBUG
            using (var cmd = new SqlCommand(String.Format("SELECT @@SPID --{0}", LoggingContext.Current.ContextGuid), databaseConnection))
            {
                cmd.CommandType = CommandType.Text;
                sqlSpId = (Int16)cmd.ExecuteScalar();
            }
#endif
        }

        private void CloseConnection()
        {
            if (databaseConnection != null)
            {
                if (databaseConnection.State != ConnectionState.Closed)
                {
                    databaseConnection.Close();
                }
                databaseConnection.Dispose();
                databaseConnection = null;
            }
        }

        private void EnsureOpenTransaction()
        {
            if (!isValid)
            {
                throw new InvalidOperationException();
            }

            if (databaseTransaction == null)
            {
                switch (transactionMode)
                {
                    case TransactionMode.AutoCommit:
                    case TransactionMode.ManualCommit:
                    case TransactionMode.DirtyRead:
                        BeginTransaction();
                        break;
                    case TransactionMode.None:
                        throw new InvalidOperationException();
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private void BeginTransaction()
        {
            EnsureOpenConnection();

            if (databaseTransaction == null)
            {
                IsolationLevel iso;
                if (transactionMode == TransactionMode.DirtyRead)
                {
                    iso = IsolationLevel.ReadUncommitted;
                }
                else
                {
                    iso = IsolationLevel.ReadCommitted;
                }

                databaseTransaction = databaseConnection.BeginTransaction(iso);
            }
        }

        /// <summary>
        /// Commits the current SQL Server transaction.
        /// </summary>
        public void CommitTransaction()
        {
            if (!isValid)
            {
                throw new InvalidOperationException();
            }

            if (databaseTransaction != null)
            {
                databaseTransaction.Commit();
                databaseTransaction.Dispose();
                databaseTransaction = null;
            }
        }

        /// <summary>
        /// Rolls back the current SQL Server transaction.
        /// </summary>
        public void RollbackTransaction()
        {
            if (!isValid)
            {
                throw new InvalidOperationException();
            }

            if (databaseTransaction != null)
            {
                databaseTransaction.Rollback();
                databaseTransaction.Dispose();
                databaseTransaction = null;
            }
        }

        public void EnsureContextEntitiesLoaded()
        {
            this.clusterReference.EnsureEntityLoaded();
            this.domainReference.EnsureEntityLoaded();
            this.federationReference.EnsureEntityLoaded();
            this.userReference.EnsureEntityLoaded();
            this.jobReference.EnsureEntityLoaded();
        }

        #region Command Creation Functions

        /// <summary>
        /// Creates a SqlCommand object with valid connection and transaction set.
        /// </summary>
        /// <param name="sql">SQL text command.</param>
        /// <returns>An initialized SqlCommand object.</returns>
        public SqlCommand CreateTextCommand(string sql)
        {
            if (!isValid)
            {
                throw new InvalidOperationException();
            }

            SqlCommand cmd = CreateCommand(sql);
            cmd.CommandType = CommandType.Text;
            return cmd;
        }

        /// <summary>
        /// Creates a SqlCommand object with valid connection and transaction set for
        /// executing a stored procedure.
        /// </summary>
        /// <param name="sql">Name of the stored procedure.</param>
        /// <returns>An initialized SqlCommand object.</returns>
        public SqlCommand CreateStoredProcedureCommand(string sql)
        {
            // TODO: caching could be done here...

            if (!isValid)
            {
                throw new InvalidOperationException();
            }

            SqlCommand cmd = CreateCommand(sql);
            cmd.CommandType = CommandType.StoredProcedure;
            return cmd;
        }

        public SqlCommand CreateCommand()
        {
            var cmd = new SqlCommand();
            cmd.Connection = DatabaseConnection;
            cmd.Transaction = DatabaseTransaction;

            return cmd;
        }

        private SqlCommand CreateCommand(string sql)
        {
            var cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Connection = DatabaseConnection;
            cmd.Transaction = DatabaseTransaction;

            return cmd;
        }

        #endregion
        #region Transaction Management Functions
        /// <summary>
        /// Sets the deadlock priority of the database transaction.
        /// </summary>
        /// <remarks>
        /// This is required for low priority job polling operations.
        /// Refer to SQL Books Online for more information on the SET DEADLOCK_PRIORITY statement.
        /// </remarks>
        /// <param name="priority">Deadlock priority of the transaction.</param>
        public void SetDeadlockPriority(DeadlockPriority priority)
        {
            if (!isValid)
            {
                throw new InvalidOperationException();
            }

            string sql = String.Format("SET DEADLOCK_PRIORITY {0}", priority.ToString());

            using (SqlCommand cmd = CreateTextCommand(sql))
            {
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Sets the deadlock priority of the database transaction.
        /// </summary>
        /// <remarks>
        /// This is required for low priority job polling operations.
        /// Refer to SQL Books Online for more information on the SET DEADLOCK_PRIORITY statement.
        /// </remarks>
        /// <param name="priority">Numeric value of deadlock priority of the transaction.</param>
        public void SetDeadlockPriority(int priority)
        {
            if (!isValid)
            {
                throw new InvalidOperationException();
            }

            string sql = String.Format("SET DEADLOCK_PRIORITY {0}", priority.ToString());

            using (SqlCommand cmd = CreateTextCommand(sql))
            {
                cmd.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
