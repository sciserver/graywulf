using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Extensions.QueryTraversal
{
    public class GraywulfSqlQueryVisitorOptions : Jhu.Graywulf.Sql.QueryTraversal.SqlQueryVisitorOptions
    {
        public GraywulfSqlQueryVisitorOptions()
        {
        }

        public GraywulfSqlQueryVisitorOptions(GraywulfSqlQueryVisitorOptions old)
            : base(old)
        {
        }
    }
}
