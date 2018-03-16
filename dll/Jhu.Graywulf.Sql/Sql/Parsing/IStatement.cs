using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public interface IStatement
    {
        bool IsResolvable { get; }

        StatementType StatementType { get; }

        IEnumerable<Statement> EnumerateSubStatements();
    }
}
