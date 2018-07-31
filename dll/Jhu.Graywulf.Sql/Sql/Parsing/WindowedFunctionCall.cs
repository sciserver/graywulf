using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class WindowedFunctionCall : IFunctionReference
    {
        public bool IsStar
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsDistinct
        {
            get { throw new NotImplementedException(); }
        }

        public OverClause OverClause
        {
            get { return FindDescendant<OverClause>(); }
        }
    }
}
