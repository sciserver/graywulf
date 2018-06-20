using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class IndexColumnDefinitionList
    {
        public IEnumerable<IndexColumnDefinition> EnumerateIndexColumns()
        {
            return EnumerateDescendants<IndexColumnDefinition>();
        }
    }
}
