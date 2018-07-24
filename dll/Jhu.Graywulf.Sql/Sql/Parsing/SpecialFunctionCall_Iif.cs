using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class SpecialFunctionCall_Iif
    {
        public Predicate Predicate
        {
            get { return FindDescendant<Predicate>(); }
        }
    }
}
