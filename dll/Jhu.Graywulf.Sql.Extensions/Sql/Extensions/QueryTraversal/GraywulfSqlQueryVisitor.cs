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

        protected override void DispatchTableSourceSpecification(Sql.Parsing.TableSourceSpecification node)
        {
            switch (node)
            {
                case PartitionedTableSourceSpecification n:
                    TraversePartitionedTableSourceSpecification(n);
                    break;
                default:
                    base.DispatchTableSourceSpecification(node);
                    break;
            }
        }

        private void TraversePartitionedTableSourceSpecification(PartitionedTableSourceSpecification node)
        {
            foreach (var nn in node.Stack)
            {
                switch (nn)
                {
                    case TableSource n:
                        DispatchTableSource(n);
                        break;
                    case TablePartitionClause n:
                        TraverseTablePartitionClause(n);
                        break;
                }
            }
        }

        protected void TraverseTablePartitionClause(TablePartitionClause node)
        {
            PushTableContext(TableContext & ~TableContext.From);

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

            PopTableContext();
        }
    }
}
