using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class InSemiJoinPredicate
    {
        public Expression Operand
        {
            get { return FindDescendant<Expression>(0); }
        }

        public SemiJoinSubquery Subquery
        {
            get { return FindDescendant<SemiJoinSubquery>(); }
        }

        public bool IsNegate
        {
            get
            {
                var not = FindDescendant<Keyword>();
                return not != null && SqlParser.ComparerInstance.Compare("NOT", not.Value) == 0;
            }
        }
    }
}
