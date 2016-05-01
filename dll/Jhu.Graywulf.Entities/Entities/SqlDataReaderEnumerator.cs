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
        private SqlDataReader reader;

        public SqlDataReaderEnumerator(SqlDataReader reader)
        {
            this.context = null;
            this.reader = reader;
        }

        public SqlDataReaderEnumerator(SqlDataReader reader, Context context)
        {
            this.context = context;
            this.reader = reader;
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
            System.Threading.Thread.Sleep(1000);

            if (reader == null)
            {
                return false;
            }
            else
            {
                var res = this.reader.Read();

                if (!res)
                {
                    Dispose();
                }

                return res;
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
