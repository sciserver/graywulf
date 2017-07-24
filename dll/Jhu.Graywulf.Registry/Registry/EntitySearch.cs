using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public class EntitySearch : ContextObject
    {
        private static readonly Regex OrderByRegex = new Regex(@"[a-z]+\s*(asc|desc){0,1}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private StringBuilder queryWhereCriteria;
        private SqlCommand queryCommand;

        private Entity parent;
        private EntityType entityType;
        private string name;
        private DeploymentState? deploymentState;
        private RunningState? runningState;

        public Entity Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        public EntityType EntityType
        {
            get { return entityType; }
            set { entityType = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public DeploymentState? DeploymentState
        {
            get { return deploymentState; }
            set { deploymentState = value; }
        }

        public RunningState? RunningState
        {
            get { return runningState; }
            set { runningState = value; }
        }

        #region Contructors and initializers

        public EntitySearch(RegistryContext context)
            : base(context)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.parent = null;
            this.name = null;
            this.deploymentState = null;
            this.runningState = null;
        }

        #endregion

        protected virtual string GetTableQuery()
        {
            string sql;

            if (!Constants.EntityTypeMap.ContainsKey(entityType))
            {
                sql = "SELECT * FROM [Entity]";
            }
            else
            {
                sql = "SELECT * FROM [Entity] INNER JOIN [{0}] ON EntityGuid = Guid";
                sql = String.Format(sql, entityType.ToString());
            }

            return sql;
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

        protected virtual void AppendParameters()
        {
        }

        protected virtual void AppendSearchCriteria()
        {
            AppendSearchCriterion("@ShowHidden = 1 OR Hidden = 0");
            AppendSearchCriterion("@ShowDeleted = 1 OR Deleted = 0");
            AppendSearchParameter("@ShowHidden", SqlDbType.Bit, RegistryContext.ShowHidden);
            AppendSearchParameter("@ShowDeleted", SqlDbType.Bit, RegistryContext.ShowDeleted);

            if (parent != null)
            {
                AppendSearchCriterion("ParentGuid = @parentGuid");
                AppendSearchParameter("@parentGuid", SqlDbType.UniqueIdentifier, parent.Guid);
            }

            if (name != null)
            {
                AppendSearchCriterion("Name LIKE @Name");
                AppendSearchParameter("@Name", SqlDbType.NVarChar, name);
            }

            if (runningState.HasValue)
            {
                AppendSearchCriterion("(RunningState & @runningState) != 0");
                AppendSearchParameter("@runningState", SqlDbType.Int, runningState.Value);
            }

            if (deploymentState.HasValue)
            {
                AppendSearchCriterion("(DeploymentState & @deploymentState) != 0");
                AppendSearchParameter("@deploymentState", SqlDbType.Int, deploymentState.Value);
            }
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

        protected void AppendSearchCriterion(string criterion)
        {
            AppendSearchCriterion(criterion, "AND");
        }

        protected void AppendSearchParameter(string name, SqlDbType type, object value)
        {
            queryCommand.Parameters.Add(name, type).Value = value;
        }

        protected void AppendSearchParameter(SqlParameter parameter)
        {
            queryCommand.Parameters.Add(parameter);
        }

        private void ValidateOrderBy(string orderBy)
        {
            // Prevent any injection attacks
            if (!String.IsNullOrWhiteSpace(orderBy) && !OrderByRegex.Match(orderBy).Success)
            {
                throw new SecurityException("Access denied."); // ****
            }
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
            return String.Format("[{0}] ASC", "Number");
        }

        public int Count()
        {
            using (queryCommand = RegistryContext.CreateCommand())
            {
                string sql = @"
{0}

WITH __e AS
(
    {1}
    {2}
)
SELECT COUNT(*) FROM __e
";

                var preamble = "";
                var table = GetTableQuery();
                var where = BuildWhereClause();

                queryCommand.CommandType = CommandType.Text;
                queryCommand.CommandText = String.Format(sql, preamble, table, where);


                AppendParameters();

                return Convert.ToInt32(queryCommand.ExecuteScalar());
            }
        }

        public IEnumerable<Entity> Find()
        {
            return Find(-1, -1, null);
        }

        public IEnumerable<Entity> Find(int max, int from, string orderBy)
        {
            ValidateOrderBy(orderBy);

            using (queryCommand = RegistryContext.CreateCommand())
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

                var preamble = "";
                var tableQuery = GetTableQuery();
                var where = BuildWhereClause();
                var orderby = BuildOrderByClause(orderBy);

                string limit;

                if (from > 0 || max > 0)
                {
                    limit = "WHERE __rn BETWEEN @__from AND @__to";
                }
                else
                {
                    limit = "";
                }

                queryCommand.CommandText = String.Format(sql, preamble, tableQuery, orderby, where, limit);

                queryCommand.Parameters.Add("@__from", SqlDbType.Int).Value = from;
                queryCommand.Parameters.Add("@__to", SqlDbType.Int).Value = from + max;

                AppendParameters();

                using (var dr = queryCommand.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Entity item;

                        if (Constants.EntityTypeMap.ContainsKey(entityType))
                        {
                            var et = (EntityType)dr.GetInt32(dr.GetOrdinal("EntityType"));
                            Type classtype;

                            if (Constants.EntityTypeMap.ContainsKey(et))
                            {
                                classtype = Constants.EntityTypeMap[et];
                            }
                            else
                            {
                                classtype = typeof(Entity);
                            }

                            item = (Entity)classtype.GetConstructor(new Type[] { typeof(RegistryContext) }).Invoke(new object[] { RegistryContext });
                        }
                        else
                        {
                            item = new Entity(RegistryContext);
                        }

                        item.LoadFromDataReader(dr);
                        yield return item;
                    }
                    dr.Close();
                }
            }
        }
    }
}
