using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UnaryOperator
    {
        public override int Precedence
        {
            get { return 1; }
        }
    }
}
