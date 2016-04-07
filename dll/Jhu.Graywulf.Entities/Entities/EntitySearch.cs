using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Entities
{
    public abstract class EntitySearch<T> : ContextObject
        where T : IDatabaseTableObject, new()
    {
        private static readonly Regex OrderByRegex = new Regex(@"[a-z]+\s*(asc|desc){0,1}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private long? id;
        private string name;
        private bool? readOnly;
        private bool? hidden;

        private StringBuilder whereCriteria;
        private SqlCommand searchCommand;

        public long? ID
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool? ReadOnly
        {
            get { return readOnly; }
            set { readOnly = value; }
        }

        public bool? Hidden
        {
            get { return hidden; }
            set { hidden = value; }
        }

        public EntitySearch()
        {
            InitializeMembers();
        }

        public EntitySearch(Context context)
            : base(context)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.readOnly = null;
            this.hidden = false;
        }

        public int Count()
        {
            using (searchCommand = Context.CreateCommand())
            {
                string sql = @"
SELECT COUNT(*) FROM ({0}) AS entities
{1}";

                var where = BuildWhereClause();
                var tableQuery = GetTableQuery();

                searchCommand.CommandText = String.Format(sql, GetTableQuery(), where);

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
                Error.AccessDenied();
            }

            using (searchCommand = Context.CreateCommand())
            {
                string sql = @"
WITH q AS
(
    SELECT entities.*, ROW_NUMBER() OVER({1}) AS rn
    FROM ({0}) AS entities
    {2}
)
SELECT * FROM q
{3}
{1}
";

                var where = BuildWhereClause();
                var orderby = BuildOrderByClause(orderBy);

                var limit = from > 0 || max > 0 ? "WHERE rn BETWEEN @from AND @to" : "";

                var table = GetTableQuery();

                searchCommand.CommandText = String.Format(sql, table, orderby, where, limit);

                searchCommand.Parameters.Add("@from", SqlDbType.Int).Value = from;
                searchCommand.Parameters.Add("@to", SqlDbType.Int).Value = from + max;

                return Context.ExecuteCommandAsEnumerable<T>(searchCommand);
            }
        }

        protected abstract string GetTableQuery();

        protected string BuildWhereClause()
        {
            whereCriteria = new StringBuilder();
            AppendSearchCriteria();

            if (whereCriteria.Length > 0)
            {
                whereCriteria.Insert(0, "WHERE ");
            }

            return whereCriteria.ToString();
        }

        protected virtual void AppendSearchCriteria()
        {
            if (id.HasValue)
            {
                AppendSearchCriterion("ID = @ID");
                AppendSearchParameter("@ID", SqlDbType.BigInt, id.Value);
            }
            else
            {
                if (name != null)
                {
                    if (name.IndexOf('%') >= 0)
                    {
                        AppendSearchCriterion("Name LIKE @Name");
                    }
                    else
                    {
                        AppendSearchCriterion("Name = @Name");
                    }

                    AppendSearchParameter("@Name", SqlDbType.NVarChar, name);
                }
            }

            if (readOnly.HasValue)
            {
                AppendSearchCriterion("ReadOnly = @ReadOnly");
                AppendSearchParameter("@ReadOnly", SqlDbType.Bit, readOnly.Value);
            }

            if (hidden.HasValue)
            {
                AppendSearchCriterion("Hidden = @Hidden");
                AppendSearchParameter("@Hidden", SqlDbType.Bit, hidden.Value);
            }
        }

        protected void AppendSearchCriterion(string criterion)
        {
            AppendSearchCriterion(criterion, "AND");
        }

        protected void AppendSearchCriterion(string criterion, string op)
        {
            if (whereCriteria.Length > 0)
            {
                whereCriteria.Append(" ");
                whereCriteria.Append(op);
                whereCriteria.Append(" ");
            }

            whereCriteria.Append("(");
            whereCriteria.Append(criterion);
            whereCriteria.AppendLine(")");
        }

        protected void AppendSearchParameter(string name, SqlDbType type, object value)
        {
            searchCommand.Parameters.Add(name, type).Value = value;
        }

        protected void AppendSearchParameter(SqlParameter parameter)
        {
            searchCommand.Parameters.Add(parameter);
        }

        protected virtual string GetDefaultOrderBy()
        {
            return "ID ASC";
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
    }
}
