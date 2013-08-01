using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Activities;
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
        /// <summary>
        /// Singleton object of <see cref="ContextManager"/> to be used for creating new <see cref="Context"/> objects.
        /// </summary>
        public static readonly ContextManager Instance = new ContextManager();  // Singleton

        #region Member Variables

        private object syncRoot = new object();

        private string connectionString;
        private string smtpString;

        private Guid userGuid;
        private string userName;

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

        /// <summary>
        /// Gets or sets the Guid of the current user.
        /// </summary>
        public Guid UserGuid
        {
            get { return userGuid; }
            set { userGuid = value; }
        }

        /// <summary>
        /// Gets or sets the Guid of the current user.
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
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
            this.connectionString = AppSettings.ConnectionString;
            this.smtpString = string.Empty;
        }

        #endregion
        #region Login functions

#if false
        public void Login()
        {
            Login(null, null);
        }

        public void Login(string userNameOrEmail, string password)
        {
            using (Context context = CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                User u = new User(context);
                bool res;

                if (userNameOrEmail == null || password == null)
                {
                    res = u.Login();
                }
                else
                {
                    res = u.Login(userNameOrEmail, password);
                }

                if (res)
                {
                    this.userGuid = u.Guid;
                    this.userName = u.Name;
                }
                else
                {
                    throw new System.UnauthorizedAccessException(ExceptionMessages.LoginFailed);
                }
            }
        }
#endif

        #endregion
        #region Context Creation Functions

        /// <summary>
        /// Creates a context with an optionally open database and SMTP connection.
        /// </summary>
        /// <param name="openConnection">True if a database connection should be opened.</param>
        /// <param name="beginTransaction">True if a transaction is required.</param>
        /// <param name="openSmtp">True if an SMTP connection should be opened.</param>
        /// <returns>A valid connection.</returns>
        public Context CreateContext(ConnectionMode connectionMode, TransactionMode transactionMode)
        {
            var context = new Context()
            {
                UserGuid = userGuid,
                UserName = userName,
                ConnectionString = connectionString,
                ConnectionMode = connectionMode,
                TransactionMode = transactionMode,
            };

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
