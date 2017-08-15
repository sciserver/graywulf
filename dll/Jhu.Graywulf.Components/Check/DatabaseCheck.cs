using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Check
{
    public class DatabaseCheck : CheckRoutineBase
    {
        public override CheckCategory Category
        {
            get
            {
                return CheckCategory.Database;
            }
        }

        public string ConnectionString { get; set; }

        public DatabaseCheck(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        protected override IEnumerable<CheckRoutineStatus> OnExecute()
        {
            var csb = new SqlConnectionStringBuilder(ConnectionString);

            yield return ReportInfo(
                "Testing database connection to server: {0}, database: {1}",
                csb.DataSource,
                csb.InitialCatalog);

            var cn = new SqlConnection(ConnectionString);
            cn.Open();

            yield return ReportSuccess("Connected. Server version: {0}", cn.ServerVersion);
        }
    }
}
