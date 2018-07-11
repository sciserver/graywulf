using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class AnyStatement
    {
        public Statement SpecificStatement
        {
            get { return (Statement)Stack.First.Value; }
        }
    }
}
