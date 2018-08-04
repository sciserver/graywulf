using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void TraversePartitionedTableSource(PartitionedTableSource node)
        {
            VisitNode(node.TableOrViewIdentifier);
            TraverseExpression(node.PartitioningKeyExpression);
            VisitNode(node);
        }
    }
}
