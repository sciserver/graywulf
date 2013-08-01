using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Logging
{
    public class WebEventFactory
    {
        private string connectionString;

        private EventFilter filter;
        private EventColumn[] orderColumns;
        private bool[] orderDirections;

        public EventFilter Filter
        {
            get { return filter; }
            set { filter = value; }
        }

        public EventColumn[] OrderColumns
        {
            get { return orderColumns; }
            set { orderColumns = value; }
        }

        public bool[] OrderDirections
        {
            get { return orderDirections; }
            set { orderDirections = value; }
        }

        public WebEventFactory()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.connectionString = AppSettings.ConnectionString;

            this.filter = new EventFilter();
            this.orderColumns = null;
            this.orderDirections = null;
        }

        public int CountEvents()
        {
            int res;

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlTransaction tn = cn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqlCommand cmd = new SqlCommand(string.Empty, cn, tn))
                    {

                        CreateCountQuery(cmd);
                        cmd.CommandType = CommandType.Text;

                        res = (int)cmd.ExecuteScalar();
                    }

                    tn.Commit();
                }
            }

            return res;
        }

        public IEnumerable<Event> SelectEvents(int from, int max)
        {
            if (orderColumns != null && orderDirections != null
                && orderColumns.Length != orderDirections.Length)
            {
                throw new ArgumentException();
            }

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlTransaction tn = cn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqlCommand cmd = new SqlCommand(string.Empty, cn, tn))
                    {

                        CreateSelectQuery(cmd, from, max);
                        cmd.CommandType = CommandType.Text;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                Event e = new Event();
                                e.LoadFromDataReader(dr);
                                yield return e;
                            }
                        }
                    }

                    tn.Commit();
                }
            }
        }

        private void CreateCountQuery(SqlCommand cmd)
        {
            string sql = "SELECT COUNT(*) FROM Event e";

            string where = filter.GenerateWhereCondition(cmd);

            if (where != null)
            {
                sql += " WHERE " + where;
            }

            cmd.CommandText = sql;
        }

        private void CreateSelectQuery(SqlCommand cmd, int from, int max)
        {
            // Get where condition
            string where = filter.GenerateWhereCondition(cmd);
            if (where != null)
            {
                where = String.Format("WHERE {0}", where);
            }
            else
            {
                where = String.Empty;
            }

            // Get order by list
            string order = String.Empty;
            if (orderColumns != null && orderDirections != null && orderColumns.Length > 0)
            {
                for (int i = 0; i < orderColumns.Length; i++)
                {
                    if (i > 0) order += ",";
                    order += orderColumns[i].ToString();
                    if (orderDirections[i]) order += " DESC";
                }
            }
            else
            {
                order = "EventID DESC";
            }

            string sql = String.Format(@"
WITH q AS
(
    SELECT e.*,
           ROW_NUMBER() OVER (ORDER BY {0}) rn
    FROM Event e
    {1} 
)
SELECT *
FROM q
WHERE rn BETWEEN @from AND @from + @max
ORDER BY {0}
",
                order,
                where);

            cmd.Parameters.Add("@from", SqlDbType.Int).Value = from;
            cmd.Parameters.Add("@max", SqlDbType.Int).Value = max;

            cmd.CommandText = sql;
        }

        public Event LoadEvent(long eventId)
        {
            var e = new Event();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlTransaction tn = cn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {
                    using (SqlCommand cmd = new SqlCommand("spGetEvent", cn, tn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@EventID", SqlDbType.BigInt).Value = eventId;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                e.LoadFromDataReader(dr);
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand("spGetEventData", cn, tn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@EventID", SqlDbType.BigInt).Value = eventId;

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                int o = 1;
                                var key = dr.GetString(o++);
                                var data = dr.GetValue(o++);

                                e.UserData.Add(key, data);
                            }
                        }
                    }


                    tn.Commit();
                }
            }

            return e;
        }
    }
}
