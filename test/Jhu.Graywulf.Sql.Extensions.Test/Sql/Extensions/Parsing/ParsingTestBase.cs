using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.Extensions.Parsing
{
    public abstract class ParsingTestBase : Jhu.Graywulf.Test.TestClassBase
    {
        protected override SqlParser CreateParser()
        {
            return new GraywulfSqlParser();
        }
    }
}
