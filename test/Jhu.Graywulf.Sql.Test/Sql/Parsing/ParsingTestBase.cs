using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public abstract class ParsingTestBase : Jhu.Graywulf.Test.TestClassBase
    {
        protected SelectStatement CreateSelect(string query)
        {
            SqlParser p = new SqlParser();
            var script = p.Execute<StatementBlock>(query);

            SqlNameResolver nr = new SqlNameResolver();
            nr.Options = new SqlNameResolverOptions()
            {
                DefaultTableDatasetName = Jhu.Graywulf.Test.Constants.TestDatasetName,
                DefaultFunctionDatasetName = Jhu.Graywulf.Test.Constants.CodeDatasetName,
                DefaultDataTypeDatasetName = Jhu.Graywulf.Test.Constants.CodeDatasetName,
            };
            nr.SchemaManager = CreateSchemaManager();
            nr.Execute(script);

            return script.FindDescendantRecursive<SelectStatement>();
        }
    }
}
