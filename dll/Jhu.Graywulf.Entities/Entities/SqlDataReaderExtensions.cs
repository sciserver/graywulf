using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Entities
{
    public static class SqlDataReaderExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this SqlDataReader reader)
            where T : IDatabaseTableObject, new()
        {
            return new SqlDataReaderEnumerator<T>(reader);
        }

        public static T AsSingleObject<T>(this SqlDataReader reader)
            where T : IDatabaseTableObject, new()
        {
            if (!reader.Read())
            {
                throw Error.NoResults(1);
            }

            var o = new T();
            o.LoadFromDataReader(reader);

            return o;
        }

        public static void AsSingleObject<T>(this SqlDataReader reader, T o)
            where T : IDatabaseTableObject
        {
            if (!reader.Read())
            {
                throw Error.NoResults(1);
            }

            o.LoadFromDataReader(reader);
        }

        public static bool TryAsSingleObject<T>(this SqlDataReader reader, T o)
            where T : IDatabaseTableObject
        {
            if (reader.Read())
            {
                o.LoadFromDataReader(reader);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int GetInt32(this SqlDataReader reader, string key)
        {
            var o = reader.GetOrdinal(key);

            if (reader.IsDBNull(o))
            {
                return -1;
            }
            else
            {
                return reader.GetInt32(o);
            }
        }

        public static double GetDouble(this SqlDataReader reader, string key)
        {
            var o = reader.GetOrdinal(key);
            return reader.GetDouble(o);
        }

        public static bool GetBoolean(this SqlDataReader reader, string key)
        {
            var o = reader.GetOrdinal(key);
            return reader.GetBoolean(o);
        }

        public static DateTime GetDateTime(this SqlDataReader reader, string key)
        {
            var o = reader.GetOrdinal(key);

            if (reader.IsDBNull(o))
            {
                return DateTime.MinValue;
            }
            else
            {
                return reader.GetDateTime(o);
            }
        }

        public static string GetString(this SqlDataReader reader, string key)
        {
            var o = reader.GetOrdinal(key);

            if (reader.IsDBNull(o))
            {
                return null;
            }
            else
            {
                return reader.GetString(o);
            }
        }
    }
}