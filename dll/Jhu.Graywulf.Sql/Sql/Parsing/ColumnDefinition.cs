using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ColumnDefinition
    {
        public ColumnDefinitionName ColumnName
        {
            get { return FindDescendant<ColumnDefinitionName>(); }
        }

        public DataTypeIdentifier DataType
        {
            get { return FindDescendant<DataTypeIdentifier>(); }
        }

        public ColumnDefaultDefinition DefaultDefinition
        {
            get { return FindDescendant<ColumnDefaultDefinition>(); }
        }

        public ColumnIdentityDefinition IdentityDefinition
        {
            get { return FindDescendant<ColumnIdentityDefinition>(); }
        }

        public ColumnConstraint Constraint
        {
            get { return FindDescendant<ColumnConstraint>(); }
        }
    }
}
