using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ColumnAlias
    {
        public static ColumnAlias Create(string alias)
        {
            return new ColumnAlias(Identifier.Create(alias));
        }
    }
}
