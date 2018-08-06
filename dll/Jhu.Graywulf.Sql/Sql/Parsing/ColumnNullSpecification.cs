using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ColumnNullSpecification
    {
        public bool IsNullable
        {
            get
            {
                foreach (var nn in Stack)
                {
                    if (nn is Literal n && SqlParser.ComparerInstance.Compare("not", n.Value) == 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
