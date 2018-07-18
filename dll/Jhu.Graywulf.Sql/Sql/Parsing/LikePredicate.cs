using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class LikePredicate
    {
        public Expression LeftOperand
        {
            get { return FindDescendant<Expression>(0); }
        }

        public Expression RightOperand
        {
            get { return FindDescendant<Expression>(1); }
        }

        public Expression EscapeOperand
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
