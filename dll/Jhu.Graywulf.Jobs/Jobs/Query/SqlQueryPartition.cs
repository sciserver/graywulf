using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Activities;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.SqlParser;
using Jhu.Graywulf.SqlCodeGen.SqlServer;

namespace Jhu.Graywulf.Jobs.Query
{
    [Serializable]
    public class SqlQueryPartition : QueryPartitionBase, ICloneable
    {
        #region Constructors and initializers

        public SqlQueryPartition()
            : base()
        {
            InitializeMembers();
        }

        public SqlQueryPartition(SqlQuery query, Context context)
            : base(query, context)
        {
            InitializeMembers();
        }

        public SqlQueryPartition(SqlQueryPartition old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(SqlQueryPartition old)
        {
        }

        public override object Clone()
        {
            return new SqlQueryPartition(this);
        }

        #endregion

        public override void PrepareExecuteQuery(Context context, IScheduler scheduler)
        {
            base.PrepareExecuteQuery(context, scheduler);

            // --- strip off orderBy clause -- it always goes into a table
            var orderby = SelectStatement.FindDescendant<OrderByClause>();
            if (orderby != null)
            {
                SelectStatement.Stack.Remove(orderby);
            }

            var qs = SelectStatement.EnumerateQuerySpecifications().First<QuerySpecification>();

            // Check if it is a partitioned query and append partitioning conditions, if necessary
            var ts = qs.EnumerateSourceTables(false).FirstOrDefault();
            if (ts != null && ts is SimpleTableSource && ((SimpleTableSource)ts).IsPartitioned)
            {
                AppendPartitioningConditions(qs, (SimpleTableSource)ts);
            }

            SubstituteDatabaseNames(AssignedServerInstance.Guid, Query.SourceDatabaseVersionName);
            SubstituteRemoteTableNames(TemporaryDatabaseInstanceReference.Value.GetDataset(), Query.TemporaryDataset.DefaultSchemaName);
        }

        private void AppendPartitioningConditions(QuerySpecification qs, SimpleTableSource ts)
        {
            if (!double.IsInfinity(PartitioningKeyFrom) || !double.IsInfinity(PartitioningKeyTo))
            {
                var cg = new SqlServerCodeGenerator();
                var column = cg.GetResolvedColumnName(ts.PartitioningColumnReference);
                var sc = GetPartitioningConditions(column, 0);

                var where = qs.FindDescendant<WhereClause>();
                if (where == null)
                {
                    where = WhereClause.Create(sc);
                    var ws = Whitespace.Create();

                    var wsn = qs.Stack.AddAfter(qs.Stack.Find(qs.FindDescendant<FromClause>()), ws);
                    qs.Stack.AddAfter(wsn, where);
                }
                else
                {
                    where.AppendCondition(sc, "AND");
                }
            }

            // --- remove partition clause
            ts.Stack.Remove(ts.FindDescendant<TablePartitionClause>());
        }

        protected override string GetExecuteQueryText()
        {
            // strip off order by
            var orderby = SelectStatement.FindDescendant<OrderByClause>();
            if (orderby != null)
            {
                SelectStatement.Stack.Remove(orderby);
            }

            // strip off partition on
            foreach (var qs in SelectStatement.EnumerateQuerySpecifications())
            {
                // strip off select into
                var into = qs.FindDescendant<IntoClause>();
                if (into != null)
                {
                    qs.Stack.Remove(into);
                }

                foreach (var ts in qs.EnumerateDescendantsRecursive<SimpleTableSource>())
                {
                    var pc = ts.FindDescendant<TablePartitionClause>();

                    if (pc != null)
                    {
                        pc.Parent.Stack.Remove(pc);
                    }
                }
            }

            // Generate code
            var sw = new StringWriter();
            var cg = new SqlServerCodeGenerator()
            {
                ResolveNames = true
            };

            cg.Execute(sw, SelectStatement);

            return sw.ToString();
        }
    }
}
