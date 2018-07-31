using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ConversionFunctionCall
    {
        public Argument Argument
        {
            get { return FindDescendant<Argument>(); }
        }

        public DataTypeSpecification DataTypeSpecification
        {
            get { return FindDescendant<DataTypeSpecification>(); }
        }
    }
}
