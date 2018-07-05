using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UpdateSetItem
    {
        public UpdateSetLeftHandSide LeftHandSide
        {
            get { return FindDescendant<UpdateSetLeftHandSide>(); }
        }

        public UpdateSetRightHandSide RightHandSide
        {
            get { return FindDescendant<UpdateSetRightHandSide>(); }
        }

        public ValueAssignmentOperator Operator
        {
            get { return FindDescendant<ValueAssignmentOperator>(); }
        }
    }
}
