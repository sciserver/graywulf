using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.CodeGeneration.SqlServer
{
    public abstract class SqlServerCodeGeneratorTestBase : Jhu.Graywulf.Schema.SqlServer.SqlServerTestBase
    {
        protected SchemaManager CreateSchemaManager()
        {
            var sm = new SqlServerSchemaManager();
            var ds = new SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.SqlServerSchemaTestConnectionString);

            sm.Datasets[ds.Name] = ds;

            return sm;
        }

        protected StatementBlock CreateScript(string query)
        {
            var p = new SqlParser();
            var script = p.Execute<StatementBlock>(query);

            SqlNameResolver nr = new SqlNameResolver();
            nr.DefaultTableDatasetName = Jhu.Graywulf.Test.Constants.TestDatasetName;
            nr.DefaultFunctionDatasetName = Jhu.Graywulf.Test.Constants.CodeDatasetName;
            nr.SchemaManager = CreateSchemaManager();
            nr.Execute(script);

            return script;
        }

        protected SelectStatement CreateSelect(string query)
        {
            return CreateScript(query).FindDescendantRecursive<SelectStatement>();
        }
    }
}
