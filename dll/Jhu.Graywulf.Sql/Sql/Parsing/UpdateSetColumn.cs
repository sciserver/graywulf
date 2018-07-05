using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UpdateSetColumn
    {
        public UpdateSetColumnLeftHandSide LeftHandSide
        {
            get { return FindDescendant<UpdateSetColumnLeftHandSide>(); }
        }

        public UpdateSetColumnRightHandSide RightHandSide
        {
            get { return FindDescendant<UpdateSetColumnRightHandSide>(); }
        }

        public ValueAssignmentOperator Operator
        {
            get { return FindDescendant<ValueAssignmentOperator>(); }
        }
    }
}
