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
    public class Context : IDisposable
    {
        #region Member Variables

        private Guid userGuid;
        private string userName;
        private Guid jobGuid;
        private string jobID;
        private Guid contextGuid;

#if DEBUG
        private int sqlSpId;
#endif

        private bool isValid;

        private bool showHidden;
        private bool showDeleted;

        private string connectionString;
        private ConnectionMode connectionMode;
        private TransactionMode transactionMode;

        private SqlConnection databaseConnection;
        private SqlTransaction databaseTransaction;

        private IGraywulfActivity activity;
        private CodeActivityContext activityContext;
        private int eventOrder;

        #endregion
        #region Member Access Properties

        /// <summary>
        /// Gets or sets the GUID of the user under who's account the operations
        /// will be executed.
        /// </summary>
        public Guid UserGuid
        {
            get { return userGuid; }
            set { userGuid = value; }
        }

        // TODO: use reference instead
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// Gets or sets the guid of the job which creates the context.
        /// </summary>
        public Guid JobGuid
        {
            get { return jobGuid; }
            set { jobGuid = value; }
        }

        public string JobID
        {
            get { return jobID; }
            set { jobID = value; }
        }

        /// <summary>
        /// Gets or sets the guid of this context.
        /// </summary>
        public Guid ContextGuid
        {
            get { return contextGuid; }
            set { contextGuid = value; }
        }

        /// <summary>
        /// Gets the validity of the context.
        /// </summary>
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

        #endregion
        #region Constructors

        /// <summary>
        /// Default constructor that creates a context object and
        /// initializes private members to their default values.
        /// </summary>
        internal Context()
        {
            InitializeMembers();
        }

        internal Context(IGraywulfActivity activity, CodeActivityContext activityContext)
        {
            InitializeMembers();

            // Get job info from the scheduler
            var scheduler = activityContext.GetExtension<IScheduler>();

            if (scheduler != null)
            {
                Guid jobguid, userguid;
                string jobid, username;

                scheduler.GetContextInfo(
                    activityContext.WorkflowInstanceId,
                    out userguid, out username,
                    out jobguid, out jobid);

                this.userGuid = userguid;
                this.userName = username;
                this.jobGuid = jobguid;
                this.jobID = jobid;

                this.contextGuid = activityContext.WorkflowInstanceId;

                this.activityContext = activityContext;
                this.activity = activity;
            }
        }

        #endregion
        #region Initializer Functions

        /// <summary>
        /// Initializes private members to their default values.
        /// </summary>
        private void InitializeMembers()
        {
            this.userGuid = Guid.Empty;
            this.userName = null;
            this.jobGuid = Guid.Empty;
            this.jobID = null;
            this.contextGuid = Guid.NewGuid();

            this.isValid = true;

            this.showHidden = false;
            this.showDeleted = false;

            this.connectionString = null;
            this.connectionMode = Registry.ConnectionMode.None;
            this.transactionMode = Registry.TransactionMode.None;

            this.databaseConnection = null;
            this.databaseTransaction = null;

            this.activity = null;
            this.eventOrder = 0;
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
                    case Registry.ConnectionMode.None:
                        throw new InvalidOperationException();
                    case Registry.ConnectionMode.AutoOpen:
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
            using (var cmd = new SqlCommand(String.Format("SELECT @@SPID --{0}", contextGuid), databaseConnection))
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
                    case Registry.TransactionMode.AutoCommit:
                    case Registry.TransactionMode.ManualCommit:
                    case Registry.TransactionMode.DirtyRead:
                        BeginTransaction();
                        break;
                    case Registry.TransactionMode.None:
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
                if (transactionMode == Registry.TransactionMode.DirtyRead)
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

            if (databaseTransaction == null)
            {
                throw new InvalidOperationException();
            }

            databaseTransaction.Commit();
            databaseTransaction.Dispose();
            databaseTransaction = null;
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

            if (databaseTransaction == null)
            {
                throw new InvalidOperationException();
            }

            databaseTransaction.Rollback();
            databaseTransaction.Dispose();
            databaseTransaction = null;
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

        private SqlCommand CreateCommand(string sql)
        {
            return new SqlCommand(sql, DatabaseConnection, DatabaseTransaction);
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
        #region Logging Support Functions

        /// <summary>
        /// Writes an event into the log database.
        /// </summary>
        /// <param name="e"></param>
        public void LogEvent(Event e)
        {
            e.UserGuid = this.userGuid;
            e.JobGuid = this.jobGuid;
            e.ContextGuid = this.contextGuid;
            e.EventSource = EventSource.Registry;
            e.ExecutionStatus = ExecutionStatus.Closed;


            // Logging is different inside activities
            if (activityContext == null)
            {
                // Direct logging invoked from code running outside a workflow.
                // Event will be written directly into the database.

                e.EventOrder = ++eventOrder;

                Logger.Instance.LogEvent(e);
            }
            else
            {
                // Logging event sent by code running inside a workflow.
                // In this case event will be routed to the workflow
                // tracking service.

                System.Activities.Tracking.CustomTrackingRecord r =
                    new System.Activities.Tracking.CustomTrackingRecord("Jhu.Graywulf.Logging");

                r.Data.Add("Jhu.Graywulf.Logging.Event", e);

                activityContext.Track(r);
            }
        }

        #endregion
        #region IDisposable Interface Implementation

        /// <summary>
        /// Disposes the context and commits the SQL transaction, closes the connection.
        /// </summary>
        public void Dispose()
        {
            if (activity != null)
            {
                activity = null;
            }

            if (databaseTransaction != null)
            {
                switch (transactionMode)
                {
                    case Registry.TransactionMode.None:
                        break;
                    case Registry.TransactionMode.AutoCommit:
                        CommitTransaction();
                        break;
                    case Registry.TransactionMode.DirtyRead:
                    case Registry.TransactionMode.ManualCommit:
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

            isValid = false;
        }

        #endregion
    }
}
