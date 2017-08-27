using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class WhileStatement : IStatement
    {
        public bool IsResolvable
        {
            get { return true; }
        }

        public BooleanExpression Condition
        {
            get
            {
                return FindDescendant<BooleanExpression>();
            }
        }

        public Statement Statement
        {
            get
            {
                return FindDescendant<Statement>();
            }
        }

        public IEnumerable<Statement> EnumerateSubStatements()
        {
            yield return Statement;
        }
    }
}
