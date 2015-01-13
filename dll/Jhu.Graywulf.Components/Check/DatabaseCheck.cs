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
        public string ConnectionString { get; set; }

        public DatabaseCheck(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public override void Execute(TextWriter output)
        {
            var csb = new SqlConnectionStringBuilder(ConnectionString);

            output.WriteLine(
                "Testing database connection to server: {0}, database: {1}",
                csb.DataSource,
                csb.InitialCatalog);

            var cn = new SqlConnection(ConnectionString);
            cn.Open();

            output.WriteLine("Connected. Server version: {0}", cn.ServerVersion);
        }
    }
}
