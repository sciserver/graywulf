using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Entities
{
    public class DataReaderEnumerator<T> : IEnumerable<T>, IEnumerator<T>
            where T : IDatabaseTableObject, new()
    {
        SqlDataReader reader;

        public DataReaderEnumerator(SqlDataReader reader)
        {
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
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            if (reader == null)
            {
                return false;
            }
            else
            {
                var res = this.reader.Read();

                if (!res)
                {
                    reader.Close();
                    reader.Dispose();
                    reader = null;
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
