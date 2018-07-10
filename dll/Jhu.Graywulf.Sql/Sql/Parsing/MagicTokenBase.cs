using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public abstract class MagicTokenBase : Node, IStatement
    {
        public bool IsResolvable
        {
            get { return false; }
        }

        public StatementType StatementType
        {
            get { return StatementType.Magic; }
        }

        public IEnumerable<Statement> EnumerateSubStatements()
        {
            yield break;
        }

        public abstract void Write(QueryRendering.QueryRendererBase renderer, TextWriter writer);
    }
}
