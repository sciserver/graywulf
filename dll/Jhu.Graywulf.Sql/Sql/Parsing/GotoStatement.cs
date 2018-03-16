using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class GotoStatement : IStatement
    {
        public bool IsResolvable
        {
            get { return false; }
        }

        public StatementType StatementType
        {
            get { return StatementType.Flow; }
        }

        public IEnumerable<Statement> EnumerateSubStatements()
        {
            yield break;
        }
    }
}
