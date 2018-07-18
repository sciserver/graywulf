using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ComparisonPredicate
    {
        public Expression LeftOperand
        {
            get { return FindDescendant<Expression>(0); }
        }

        public Expression RightOperand
        {
            get { return FindDescendant<Expression>(1); }
        }

        public ComparisonOperator Operator
        {
            get { return FindDescendant<ComparisonOperator>(); }
        }
    }
}
