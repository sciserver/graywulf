using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;


namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UpdateStatement : IStatement
    {
        public bool IsResolvable
        {
            get { return true; }
        }

        public IEnumerable<Statement> EnumerateSubStatements()
        {
            yield break;
        }
    }
}
