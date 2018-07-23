using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public class CnfConverter : NfConverter
    {
        protected internal override ExpressionTreeNode VisitOperatorOr(OperatorOr node)
        {
            // Both operands must be in CNF
            var left = Visit(node.Left);
            var right = Visit(node.Right);

            // Form Cartesian product of terms
            ExpressionTreeNode[] leftterms, rightterms;

            if (left is OperatorAnd)
            {
                leftterms = ((OperatorAnd)left).EnumerateTerms().ToArray();
            }
            else
            {
                leftterms = new ExpressionTreeNode[] { left };
            }

            if (right is OperatorAnd)
            {
                rightterms = ((OperatorAnd)right).EnumerateTerms().ToArray();
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
                    var t = new OperatorOr(leftterms[i], rightterms[j]);

                    if (res == null)
                    {
                        res = t;
                    }
                    else
                    {
                        res = new OperatorAnd(t, res);
                    }
                }
            }

            return res;
        }
    }
}
