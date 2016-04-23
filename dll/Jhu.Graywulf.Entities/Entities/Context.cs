using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;
using Jhu.Graywulf.AccessControl;

namespace Jhu.Graywulf.Entities
{
    public class Context : IDisposable
    {
        private bool isValid;
        private string connectionString;
        private SqlConnection connection;
        private SqlTransaction transaction;
        private Principal principal;

        #region Properties

        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        public SqlConnection Connection
        {
            get
            {
                if (connection == null)
                {
                    OpenConnection();
                }

                return connection;
            }
        }

        public SqlTransaction Transaction
        {
            get
            {
                if (transaction == null)
                {
                    BeginTransaction();
                }

                return transaction;
            }
        }

        public Principal Principal
        {
            get { return principal; }
            set { principal = value; }
        }

        #endregion
        #region Constructors and initializers

        public Context()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.isValid = true;
            this.connectionString = null;
            this.connection = null;
            this.transaction = null;
            this.principal = null;
        }

        public void Dispose()
        {
            if (transaction != null)
            {
                CommitTransaction();
            }

            if (connection != null)
            {
                CloseConnection();
            }

            isValid = false;
        }

        #endregion

        private void EnsureOpenConnection()
        {
            if (connection == null)
            {
                OpenConnection();
            }
        }

        public SqlConnection OpenConnection()
        {
            if (!isValid)
            {
                throw Error.ContextInvalid();
            }

            if (connection != null)
            {
                throw Error.ConnectionAlreadyOpen();
            }

            connection = new SqlConnection(connectionString);
            connection.Open();

            return connection;
        }

        private void CloseConnection()
        {
            if (connection != null)
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
                connection.Dispose();
                connection = null;
            }
        }

        private void EnsureOpenTransaction()
        {
            if (transaction == null)
            {
                BeginTransaction();
            }
        }

        private SqlTransaction BeginTransaction()
        {
            if (!isValid)
            {
                throw Error.ContextInvalid();
            }

            if (transaction != null)
            {
                throw Error.TransactionAlreadyOpen();
            }

            if (transaction == null)
            {
                transaction = connection.BeginTransaction();
            }

            return transaction;
        }

        /// <summary>
        /// Commits the current SQL Server transaction.
        /// </summary>
        public void CommitTransaction()
        {
            if (!isValid)
            {
                throw Error.ContextInvalid();
            }

            if (transaction == null)
            {
                throw Error.TransactionNotOpen();
            }

            transaction.Commit();
            transaction.Dispose();
            transaction = null;
        }

        /// <summary>
        /// Rolls back the current SQL Server transaction.
        /// </summary>
        public void RollbackTransaction()
        {
            if (!isValid)
            {
                throw Error.ContextInvalid();
            }

            if (transaction == null)
            {
                throw Error.TransactionNotOpen();
            }

            transaction.Rollback();
            transaction.Dispose();
            transaction = null;
        }

        #region Command helpers

        public SqlCommand CreateCommand()
        {
            return new SqlCommand()
            {
                CommandType = CommandType.Text,
            };
        }

        public SqlCommand CreateCommand(string sql)
        {
            return new SqlCommand()
            {
                CommandText = sql,
                CommandType = CommandType.Text,
            };
        }

        public SqlCommand CreateStoredProcedureCommand(string sql)
        {
            return new SqlCommand()
            {
                CommandText = sql,
                CommandType = CommandType.StoredProcedure,
            };
        }

        #endregion
        #region Query execution helpers

        private void PrepareCommand(SqlCommand cmd)
        {
            PrepareCommand(cmd, true);
        }

        private void PrepareCommand(SqlCommand cmd, bool transaction)
        {
            EnsureOpenConnection();
            cmd.Connection = connection;

            if (transaction)
            {
                EnsureOpenTransaction();
                cmd.Transaction = this.transaction;
            }
            
            cmd.CommandTimeout = 30;        // TODO: from settings
        }

        public IEnumerable<T> ExecuteCommandAsEnumerable<T>(SqlCommand cmd)
            where T : IDatabaseTableObject, new()
        {
            PrepareCommand(cmd);

            var dr = cmd.ExecuteReader();

            return dr.AsEnumerable<T>();
        }

        public T ExecuteCommandAsSingleObject<T>(SqlCommand cmd)
            where T : IDatabaseTableObject, new()
        {
            PrepareCommand(cmd);

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                var o = dr.AsSingleObject<T>();

                if (o is ContextObject)
                {
                    ((ContextObject)(object)o).Context = this;
                }

                return o;
            }
        }

        public void ExecuteCommandAsSingleObject<T>(SqlCommand cmd, T o)
            where T : IDatabaseTableObject
        {
            PrepareCommand(cmd);

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                dr.AsSingleObject<T>(o);
            }
        }

        public bool TryExecuteCommandAsSingleObject<T>(SqlCommand cmd, T o)
            where T : IDatabaseTableObject
        {
            PrepareCommand(cmd);

            using (var dr = cmd.ExecuteReader(CommandBehavior.SingleRow))
            {
                return dr.TryAsSingleObject<T>(o);
            }
        }

        public SqlDataReader ExecuteCommandReader(SqlCommand cmd)
        {
            PrepareCommand(cmd);

            return cmd.ExecuteReader();
        }

        public object ExecuteCommandNonQuery(SqlCommand cmd)
        {
            PrepareCommand(cmd);

            cmd.ExecuteNonQuery();
            
            return cmd.Parameters["RETVAL"].Value;
        }

        public object ExecuteCommandScalar(SqlCommand cmd)
        {
            PrepareCommand(cmd);

            var res = cmd.ExecuteScalar();

            return res;
        }

        public void ExecuteScriptNonQuery(string sql)
        {
            var parts = SplitQuery(sql);

            for (int i = 0; i < parts.Length; i++)
            {
                using (var cmd = CreateCommand(parts[i]))
                {
                    PrepareCommand(cmd, false);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public string[] SplitQuery(string sql)
        {
            int i = 0;
            var res = new List<string>();

            while (true)
            {
                // Find the next GO statement
                var j = sql.IndexOf("GO", i, StringComparison.InvariantCultureIgnoreCase);

                if (j < 0)
                {
                    AddToScriptIfValid(res, sql.Substring(i));
                    break;
                }
                else
                {
                    AddToScriptIfValid(res, sql.Substring(i, j - i));
                }

                i = j + 2;
            }

            return res.ToArray();
        }

        private void AddToScriptIfValid(List<string> script, string sql)
        {
            if (!String.IsNullOrWhiteSpace(sql))
            {
                script.Add(sql.Trim());
            }
        }

        #endregion
    }
}
