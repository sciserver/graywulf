using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UpdateSetColumnLeftHandSide
    {
        public UserVariable UserVariable
        {
            get { return FindDescendant<UserVariable>(); }
        }

        public ColumnIdentifier ColumnIdentifier
        {
            get { return FindDescendant<ColumnIdentifier>(); }
        }
    }
}
