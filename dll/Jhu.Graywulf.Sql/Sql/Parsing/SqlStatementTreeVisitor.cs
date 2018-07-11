using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.QueryRewriting;

namespace Jhu.Graywulf.Sql.Parsing
{
    /// <summary>
    /// Implements a generic SQL tree traversal algorithm to be used with name
    /// resolution and query rewriting
    /// </summary>
    public abstract class SqlStatementTreeVisitor
    {

        protected virtual void VisitStatementBlock(StatementBlock node)
        {
            foreach (var statement in node.EnumerateDescendants<AnyStatement>(true))
            {
                //ResolveStatement(statement);
            }
        }

    }
}
