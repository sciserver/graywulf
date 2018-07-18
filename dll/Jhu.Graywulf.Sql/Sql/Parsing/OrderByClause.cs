using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class OrderByClause
    {
        public ArgumentList ArgumentList
        {
            get { return FindDescendant<ArgumentList>(); }
        }

        // TODO: there's a bunch of new features (FETCH syntax)
    }
}
