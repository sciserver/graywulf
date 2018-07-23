using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public class DnfConverter : NfConverter
    {
        protected internal override ExpressionTreeNode VisitOperatorAnd(OperatorAnd node)
        {
            // Both operands must be in DNF
            var left = Visit(node.Left);
            var right = Visit(node.Right);

            // Form Cartesian product of terms
            ExpressionTreeNode[] leftterms, rightterms;

            if (left is OperatorOr)
            {
                leftterms = ((OperatorOr)left).EnumerateTerms().ToArray();
            }
            else
            {
                leftterms = new ExpressionTreeNode[] { left };
            }

            if (right is OperatorOr)
            {
                rightterms = ((OperatorOr)right).EnumerateTerms().ToArray();
            }
            else
            {
                rightterms = new ExpressionTreeNode[] { right };
            }

            ExpressionTreeNode res = null;

            for (int i = 0; i < leftterms.Length; i++)
            {
                for (int j = 0; j < rightterms.Length; j++)
                {
                    var t = new OperatorAnd(leftterms[i], rightterms[j]);

                    if (res == null)
                    {
                        res = t;
                    }
                    else
                    {
                        res = new OperatorOr(t, res);
                    }
                }
            }

            return res;
        }
    }
}
