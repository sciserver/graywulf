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
        public FunctionIdentifier UdfIdentifier
        {
            get { return FindDescendant<FunctionIdentifier>(); }
        }

        public FunctionReference FunctionReference
        {
            get { return UdfIdentifier.FunctionReference; }
            set { UdfIdentifier.FunctionReference = value; }
        }
    }
}
