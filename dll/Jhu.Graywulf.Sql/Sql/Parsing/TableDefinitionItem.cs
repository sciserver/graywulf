using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class TableDefinitionItem
    {
        public ColumnDefinition ColumnDefinition
        {
            get { return FindDescendant<ColumnDefinition>(); }
        }

        public TableConstraint TableConstraint
        {
            get { return FindDescendant<TableConstraint>(); }
        }

        public TableIndex TableIndex
        {
            get { return FindDescendant<TableIndex>(); }
        }
    }
}
