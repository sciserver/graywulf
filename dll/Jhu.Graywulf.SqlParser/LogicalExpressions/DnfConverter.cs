using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser.LogicalExpressions
{
    public class DnfConverter : NfConverter
    {
        protected internal override Expression VisitOperatorAnd(OperatorAnd node)
        {
            // Both operands must be in DNF
            var left = Visit(node.Left);
            var right = Visit(node.Right);

            // Form Cartesian product of terms
            Expression[] leftterms, rightterms;

            if (left is OperatorOr)
            {
                leftterms = ((OperatorOr)left).EnumerateTerms().ToArray();
            }
            else
            {
                leftterms = new Expression[] { left };
            }

            if (right is OperatorOr)
            {
                rightterms = ((OperatorOr)right).EnumerateTerms().ToArray();
            }
            else
            {
                rightterms = new Expression[] { right };
            }

            Expression res = null;

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
