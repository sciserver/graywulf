using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class BetweenPredicate
    {
        public Expression LeftOperand
        {
            get { return FindDescendant<Expression>(0); }
        }

        public Expression StartOperand
        {
            get { return FindDescendant<Expression>(1); }
        }

        public Expression EndOperand
        {
            get { return FindDescendant<Expression>(2); }
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
