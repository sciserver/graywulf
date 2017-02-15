using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
namespace Jhu.Graywulf.Schema.SqlServer
{
    public class SqlServerTestBase: Jhu.Graywulf.Test.TestClassBase
    {

        protected SqlServerDataset CreateTestDataset()
        {
            var csb = new SqlConnectionStringBuilder(Jhu.Graywulf.Schema.Test.AppSettings.SqlServerConnectionString);

            var ds = new SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, csb.ConnectionString)
            {
                DatabaseName = csb.InitialCatalog
            };

            return ds;
        }
    }
}
