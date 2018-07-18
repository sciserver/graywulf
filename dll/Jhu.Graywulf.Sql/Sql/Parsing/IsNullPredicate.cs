using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class IsNullPredicate
    {
        public Expression Operand
        {
            get { return FindDescendant<Expression>(0); }
        } 

        public bool IsNegate
        {
            get
            {
                var not = FindDescendant<Keyword>(1);
                return not != null && SqlParser.ComparerInstance.Compare("NOT", not.Value) == 0;
            }
        }
    }
}
