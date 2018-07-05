using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UpdateSetColumnRightHandSide
    {
        public bool isDefault
        {
            get { return Stack.First.Value is Keyword; }
        }

        public Expression Expression
        {
            get { return FindDescendant<Expression>(); }
        }
    }
}
