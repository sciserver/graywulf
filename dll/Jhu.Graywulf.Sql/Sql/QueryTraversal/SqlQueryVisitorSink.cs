using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.QueryTraversal
{
    public abstract class SqlQueryVisitorSink
    {
        protected internal abstract void AcceptVisitor(SqlQueryVisitor visitor, Token node);
        protected internal abstract void AcceptVisitor(SqlQueryVisitor visitor, IDatabaseObjectReference node);
    }
}
