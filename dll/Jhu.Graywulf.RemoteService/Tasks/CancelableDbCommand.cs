using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Tasks
{
    public class CancelableDbCommand : CancelableTask
    {
        [NonSerialized]
        private IDbCommand command;

        public CancelableDbCommand(IDbCommand command)
        {
            InitializeMembers();

            this.command = command;
        }

        private void InitializeMembers()
        {
            this.command = null;
        }

        public int ExecuteNonQuery()
        {
            try
            {
                return command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ProcessException(ex);
            }
        }

        public object ExecuteScalar()
        {
            try
            {
                return command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ProcessException(ex);
            }
        }

        public void ExecuteReader(Action<IDataReader> action)
        {
            try
            {
                using (var dr = command.ExecuteReader())
                {
                    action(dr);
                }
            }
            catch (Exception ex)
            {
                throw ProcessException(ex);
            }
        }

        protected override void OnExecute()
        {
            try
            {
                ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ProcessException(ex);
            }
        }

        public override void EndExecute()
        {
            try
            {
                base.EndExecute();
            }
            catch (Exception ex)
            {
                throw ProcessException(ex);
            }
        }

        public override void Cancel()
        {
            base.Cancel();

            command.Cancel();
        }

        private Exception ProcessException(Exception exception)
        {
            if (exception is SqlException)
            {
                var ex = (SqlException)exception;
                if (ex.ErrorCode == -2146232060 && ex.Class == 11)
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
