using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.QueryTraversal;
using Jhu.Graywulf.Sql.Extensions.Parsing;

namespace Jhu.Graywulf.Sql.Extensions.QueryTraversal
{
    public class GraywulfSqlQueryVisitor : SqlQueryVisitor
    {
        #region Properties

        public new GraywulfSqlQueryVisitorOptions Options
        {
            get { return (GraywulfSqlQueryVisitorOptions)base.Options; }
            set { base.Options = value; }
        }

        #endregion
        #region Constructors and initializers

        public GraywulfSqlQueryVisitor(SqlQueryVisitorSink sink)
            :base(sink)
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        protected override SqlQueryVisitorOptions CreateOptions()
        {
            return new GraywulfSqlQueryVisitorOptions();
        }

        #endregion
        
        protected override void DispatchTableSource(TableSource node)
        {
            switch (node)
            {
                case PartitionedTableSource n:
                    TraversePartitionedTableSource(n);
                    break;
                default:
                    base.DispatchTableSource(node);
                    break;
            }
        }

        protected void TraversePartitionedTableSource(PartitionedTableSource node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case TableOrViewIdentifier n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                    case TableAlias n:
                        VisitNode(n);
                        break;
                    case TableSampleClause n:
                        TraverseTableSampleClause(n);
                        break;
                    case TableHintClause n:
                        TraverseTableHintClause(n);
                        break;
                    case TablePartitionClause n:
                        TraverseTablePartitionClause(n);
                        break;
                }
            }

            VisitNode(node);
        }

        protected void TraverseTablePartitionClause(TablePartitionClause node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case Literal n:
                        VisitNode(n);
                        break;
                    case ColumnIdentifier n:
                        VisitNode(n);
                        VisitReference(n);
                        break;
                }
            }

            VisitNode(node);
        }
    }
}
