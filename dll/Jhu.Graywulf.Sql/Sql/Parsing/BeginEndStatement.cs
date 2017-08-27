using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class BeginEndStatement : IStatement
    {
        public bool IsResolvable
        {
            get { return false; }
        }

        public StatementBlock StatementBlock
        {
            get { return FindDescendant<StatementBlock>(); }
        }

        public IEnumerable<Statement> EnumerateSubStatements()
        {
            return StatementBlock.EnumerateSubStatements();
        }
    }
}
