using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;

namespace Jhu.Graywulf.Sql.Schema.SqlServer
{
    public class SqlServerTestBase: Jhu.Graywulf.Test.TestClassBase
    {

        protected SqlServerDataset CreateTestDataset()
        {
            var csb = new SqlConnectionStringBuilder(Jhu.Graywulf.Sql.Schema.AppSettings.SqlServerConnectionString);

            var ds = new SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, csb.ConnectionString)
            {
                DatabaseName = csb.InitialCatalog
            };

            return ds;
        }
    }
}
