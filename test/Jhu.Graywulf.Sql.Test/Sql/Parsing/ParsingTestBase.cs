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
    public abstract class ParsingTestBase
    {
        protected SchemaManager CreateSchemaManager()
        {
            var sm = new SqlServerSchemaManager();
            var ds = new SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.SqlServerSchemaTestConnectionString);

            sm.Datasets[ds.Name] = ds;

            return sm;
        }

        protected SelectStatement CreateSelect(string query)
        {
            SqlParser p = new SqlParser();
            var script = p.Execute<StatementBlock>(query);

            SqlNameResolver nr = new SqlNameResolver();
            nr.DefaultTableDatasetName = Jhu.Graywulf.Test.Constants.TestDatasetName;
            nr.DefaultFunctionDatasetName = Jhu.Graywulf.Test.Constants.CodeDatasetName;
            nr.DefaultDataTypeDatasetName = Jhu.Graywulf.Test.Constants.CodeDatasetName;
            nr.SchemaManager = CreateSchemaManager();
            nr.Execute(script);

            return script.FindDescendantRecursive<SelectStatement>();
        }
    }
}
