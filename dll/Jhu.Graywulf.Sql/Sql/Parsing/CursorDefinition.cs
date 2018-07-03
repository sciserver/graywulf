using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class CursorDefinition
    {
        public SelectStatement SelectStatement
        {
            get { return FindDescendant<SelectStatement>(); }
        }
    }
}
