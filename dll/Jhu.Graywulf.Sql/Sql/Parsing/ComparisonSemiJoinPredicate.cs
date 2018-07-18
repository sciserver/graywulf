using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ComparisonSemiJoinPredicate
    {
        public Expression Operand
        {
            get { return FindDescendant<Expression>(0); }
        }

        public ComparisonOperator Operator
        {
            get { return FindDescendant<ComparisonOperator>(); }
        }

        public string Method
        {
            get { return FindDescendant<Keyword>().Value.ToUpperInvariant(); }
        }

        public SemiJoinSubquery Subquery
        {
            get { return FindDescendant<SemiJoinSubquery>(); }
        }
    }
}
