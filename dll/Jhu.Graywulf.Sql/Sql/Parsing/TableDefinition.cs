using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class TableDefinition
    {
        public TableDefinitionList TableDefinitionList
        {
            get { return FindDescendant<TableDefinitionList>(); }
        }
    }
}
