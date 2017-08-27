using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;


namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class DeleteStatement : IStatement
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
