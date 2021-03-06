﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Principal;
using System.IO;

namespace Jhu.Graywulf.Entities
{
    public class Context : IDisposable
    {

#if DEBUG
        private StringWriter connectionMessages;
#endif

        private bool isValid;
        private bool autoDispose;
        private string connectionString;
        private SqlConnection connection;
        private SqlTransaction transaction;
        private IPrincipal principal;

        #region Properties

        public bool AutoDispose
        {
            get { return autoDispose; }
            set { autoDispose = value; }
        }

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

        public IPrincipal Principal
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

#if DEBUG
            connectionMessages = new StringWriter();
            connection.InfoMessage += Connection_InfoMessage;
#endif

            return connection;
        }

#if DEBUG
        private void Connection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {
            connectionMessages.WriteLine(e.Message);
        }
#endif

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

#if DEBUG
            if (connectionMessages != null)
            {
                connectionMessages.Dispose();
                connectionMessages = null;
            }
#endif
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
            return new SqlDataReaderEnumerator<T>(cmd, this);
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

            if (cmd.Parameters.Contains("RETVAL"))
            {
                return cmd.Parameters["RETVAL"].Value;
            }
            else
            {
                return null;
            }
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
