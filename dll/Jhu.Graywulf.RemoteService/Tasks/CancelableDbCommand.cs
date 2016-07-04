using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Tasks
{
    /// <summary>
    /// Implements a wrapper around a DbCommand to support
    /// cancelling of long-running queries.
    /// </summary>
    public class CancelableDbCommand : CancelableTask
    {
        #region Private members

        /// <summary>
        /// Holds a reference to the executing command.
        /// </summary>
        [NonSerialized]
        private IDbCommand command;

        #endregion
        #region Constructors and initializers

        public CancelableDbCommand(IDbCommand command)
        {
            InitializeMembers();

            this.command = command;
        }

        private void InitializeMembers()
        {
            this.command = null;
        }

        #endregion

        /// <summary>
        /// Executes the command synchronously by returning the number of affected
        /// rows only.
        /// </summary>
        /// <returns></returns>
        public int ExecuteNonQuery()
        {
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw DispatchException(ex);
            }
        }

        /// <summary>
        /// Executes the command synchronously by returning a scalar only.
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar()
        {
            try
            {
                return command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw DispatchException(ex);
            }
        }

        public long ExecuteReader(Action<IDataReader> action)
        {
            return ExecuteReader(CommandBehavior.Default, action);
        }

        /// <summary>
        /// Executes the query synchronously by passing the resulting
        /// data reader to a delegate.
        /// </summary>
        /// <param name="action"></param>
        public long ExecuteReader(CommandBehavior behavior, Action<IDataReader> action)
        {
            try
            {
                using (var dr = command.ExecuteReader(behavior))
                {
                    action(dr);
                    return dr.RecordsAffected;
                }
            }
            catch (Exception ex)
            {
                throw DispatchException(ex);
            }
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <remarks>
        /// This function is called by the infrastructure when executing
        /// the task asynchronously.
        /// </remarks>
        protected override void OnExecute()
        {
            try
            {
                ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw DispatchException(ex);
            }
        }

        /// <summary>
        /// Waits for the asynchronous task to complete.
        /// </summary>
        public override void EndExecute()
        {
            try
            {
                base.EndExecute();
            }
            catch (Exception ex)
            {
                throw DispatchException(ex);
            }
        }

        /// <summary>
        /// Cancels the command.
        /// </summary>
        public override void Cancel()
        {
            base.Cancel();

            command.Cancel();
        }

        /// <summary>
        /// Processes exceptions
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        private Exception DispatchException(Exception exception)
        {
            // When a command is canceled, a SqlException is thrown by the
            // client. Handle this and throw an OperationCanceledException instead.

            if (exception is SqlException)
            {
                var ex = (SqlException)exception;

                if (ex.Class == 11 && ex.Number == -2)
                {
                    return new TimeoutException(ex.Message, ex);
                }
                if (ex.Class == 11 && ex.Number == 0 && ex.ErrorCode == -2146232060)
                {
                    return new OperationCanceledException(ex.Message, ex);
                }
            }

            return exception;

            // TODO: cancel exception probably differs from provider to provider
            // figure out what to do
        }
    }
}
