using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Entities.Mapping;

namespace Jhu.Graywulf.Entities
{
    public abstract class EntitySearch<T> : ContextObject
        where T : Entity, new()
    {
        private static readonly Regex OrderByRegex = new Regex(@"[a-z]+\s*(asc|desc){0,1}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private DbTable dbTable_cache;
        private DbTable searchDbTable_cache;

        private StringBuilder queryPreamble;
        private StringBuilder queryWhereCriteria;
        private SqlCommand searchCommand;

        private DbTable DbTable
        {
            get
            {
                if (dbTable_cache == null)
                {
                    var t = this.GetType();

                    while (t != typeof(object))
                    {
                        if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(EntitySearch<>))
                        {
                            t = t.GetGenericArguments()[0];
                            break;
                        }

                        t = t.BaseType;
                    }

                    if (t == typeof(object))
                    {
                        throw DbError.NoTableClassFound(this.GetType());
                    }

                    dbTable_cache = DbTable.GetDbTable(t);
                }

                return dbTable_cache;
            }
        }

        private DbTable SearchDbTable
        {
            get
            {
                if (searchDbTable_cache == null)
                {
                    searchDbTable_cache = DbTable.GetDbTable(this.GetType());
                }

                return searchDbTable_cache;
            }
        }

        protected EntitySearch()
        {
            InitializeMembers();
        }

        protected EntitySearch(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        public int Count()
        {
            using (searchCommand = Context.CreateCommand())
            {
                string sql = @"
{0}

WITH __e AS
(
{1}
)
SELECT COUNT(*)
FROM __e
{2}";

                var t = new T();
                var preamble = BuildPreamble();
                var tableQuery = t.GetTableQuery();
                var where = BuildWhereClause();

                searchCommand.CommandText = String.Format(sql, preamble, tableQuery, where);

                return Convert.ToInt32(Context.ExecuteCommandScalar(searchCommand));
            }
        }

        public IEnumerable<T> Find()
        {
            return Find(-1, -1, null);
        }

        public IEnumerable<T> Find(int max, int from, string orderBy)
        {
            // Prevent any injection attacks
            if (!String.IsNullOrWhiteSpace(orderBy) && !OrderByRegex.Match(orderBy).Success)
            {
                AccessControl.Error.AccessDenied();
            }

            using (searchCommand = Context.CreateCommand())
            {
                string sql = @"
{0}

WITH 
__e AS
(
    {1}
),
__r AS
(
    SELECT __e.*, ROW_NUMBER() OVER({2}) AS __rn
    FROM __e
    {3}
)
SELECT __r.* FROM __r
{4}
ORDER BY __rn
";

                var t = new T();
                var preamble = BuildPreamble();
                var tableQuery = t.GetTableQuery();
                var where = BuildWhereClause();
                var orderby = BuildOrderByClause(orderBy);

                string limit;

                if (from > 0 || max > 0)
                {
                    limit = "WHERE __rn BETWEEN " + Constants.FromParameterName + " AND " + Constants.ToParameterName;
                }
                else
                {
                    limit = "";
                }

                searchCommand.CommandText = String.Format(sql, preamble, tableQuery, orderby, where, limit);

                searchCommand.Parameters.Add(Constants.FromParameterName, SqlDbType.Int).Value = from;
                searchCommand.Parameters.Add(Constants.ToParameterName, SqlDbType.Int).Value = from + max;

                return Context.ExecuteCommandAsEnumerable<T>(searchCommand);
            }
        }

        protected string BuildPreamble()
        {
            queryPreamble = new StringBuilder();

            AppendPreambleItems();

            return queryPreamble.ToString();
        }

        protected virtual void AppendPreambleItems()
        {
        }

        protected void AppendPreambleItem(string sql)
        {
            queryPreamble.AppendLine(sql);
        }

        protected string BuildWhereClause()
        {
            queryWhereCriteria = new StringBuilder();
            AppendSearchCriteria();

            if (queryWhereCriteria.Length > 0)
            {
                queryWhereCriteria.Insert(0, "WHERE ");
            }

            return queryWhereCriteria.ToString();
        }

        protected virtual void AppendSearchCriteria()
        {
            foreach (var c in SearchDbTable.Columns.Values)
            {
                string criterion;
                SqlParameter[] parameters;

                if (c.GetSearchCriterion(this, out criterion, out parameters))
                {
                    bool cancel = false;

                    OnAppendingSearchCriterion(c, ref criterion, ref parameters, ref cancel);

                    if (!cancel)
                    {
                        AppendSearchCriterion(criterion);

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            if (parameters[i] != null)
                            {
                                AppendSearchParameter(parameters[i]);
                            }
                        }
                    }
                }
            }
        }

        protected virtual void OnAppendingSearchCriterion(DbColumn column, ref string criterion, ref SqlParameter[] parameters, ref bool cancel)
        {
        }

        protected void AppendSearchCriterion(string criterion)
        {
            AppendSearchCriterion(criterion, "AND");
        }

        protected void AppendSearchCriterion(string criterion, string op)
        {
            if (queryWhereCriteria.Length > 0)
            {
                queryWhereCriteria.Append(" ");
                queryWhereCriteria.Append(op);
                queryWhereCriteria.Append(" ");
            }

            queryWhereCriteria.Append("(");
            queryWhereCriteria.Append(criterion);
            queryWhereCriteria.AppendLine(")");
        }

        protected void AppendSearchParameter(string name, SqlDbType type, object value)
        {
            searchCommand.Parameters.Add(name, type).Value = value;
        }

        protected void AppendSearchParameter(SqlParameter parameter)
        {
            searchCommand.Parameters.Add(parameter);
        }

        protected string BuildOrderByClause(string orderBy)
        {
            if (!String.IsNullOrWhiteSpace(orderBy))
            {
                return String.Format("ORDER BY {0}", orderBy);
            }
            else
            {
                return "ORDER BY " + GetDefaultOrderBy();
            }
        }

        protected virtual string GetDefaultOrderBy()
        {
            return String.Format("[{0}] ASC", DbTable.Key.Name);
        }
    }
}
