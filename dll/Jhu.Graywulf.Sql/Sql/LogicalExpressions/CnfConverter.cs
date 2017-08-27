using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public class CnfConverter : NfConverter
    {
        protected internal override Expression VisitOperatorOr(OperatorOr node)
        {
            // Both operands must be in CNF
            var left = Visit(node.Left);
            var right = Visit(node.Right);

            // Form Cartesian product of terms
            Expression[] leftterms, rightterms;

            if (left is OperatorAnd)
            {
                leftterms = ((OperatorAnd)left).EnumerateTerms().ToArray();
            }
            else
            {
                leftterms = new Expression[] { left };
            }

            if (right is OperatorAnd)
            {
                rightterms = ((OperatorAnd)right).EnumerateTerms().ToArray();
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
