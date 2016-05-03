using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Entities
{
    public class SqlDataReaderEnumerator<T> : IEnumerable<T>, IEnumerator<T>
            where T : IDatabaseTableObject, new()
    {
        private Context context;
        private SqlCommand command;
        private SqlDataReader reader;

        public SqlDataReaderEnumerator(SqlCommand command)
        {
            this.context = null;
            this.command = command;
            this.reader = null;
        }

        public SqlDataReaderEnumerator(SqlCommand command, Context context)
        {
            this.context = context;
            this.command = command;
            this.reader = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public T Current
        {
            get
            {
                if (reader == null)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    var value = new T();
                    value.LoadFromDataReader(reader);
                    return value;
                }
            }
        }

        public void Dispose()
        {
            if (reader != null)
            {
                reader.Close();
                reader.Dispose();
                reader = null;
            }

            if (command != null)
            {
                command.Dispose();
                command = null;
            }

            if (context != null && context.AutoDispose)
            {
                context.Dispose();
                context = null;
            }
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            if (reader == null)
            {
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
            }

            var res = this.reader.Read();

            if (!res)
            {
                Dispose();
            }

            return res;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
