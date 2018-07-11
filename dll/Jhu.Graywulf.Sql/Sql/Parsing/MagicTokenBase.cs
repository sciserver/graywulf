using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public abstract class MagicTokenBase : Statement
    {
        public override IEnumerable<AnyStatement> EnumerateSubStatements()
        {
            yield break;
        }

        public abstract void Write(QueryRendering.QueryRendererBase renderer, TextWriter writer);
    }
}
