using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public abstract partial class Statement
    {
        public abstract IEnumerable<AnyStatement> EnumerateSubStatements();
    }
}
