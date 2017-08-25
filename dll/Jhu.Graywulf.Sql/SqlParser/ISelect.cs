using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.SqlParser
{
    public interface ISelect
    {
        QueryExpression QueryExpression { get; }

        OrderByClause OrderByClause { get; }
    }
}
