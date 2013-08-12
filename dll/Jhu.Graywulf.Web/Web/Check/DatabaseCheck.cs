using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Web;

namespace Jhu.Graywulf.Web.Check
{
    public class DatabaseCheck : CheckRoutineBase
    {
        public string ConnectionString { get; set; }

        public DatabaseCheck(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public override void Execute(PageBase page)
        {
            var csb = new SqlConnectionStringBuilder(ConnectionString);

            page.Response.Output.WriteLine(
                "Testing database connection to server: {0}, database: {1}",
                csb.DataSource,
                csb.InitialCatalog);

            var cn = new SqlConnection(ConnectionString);
            cn.Open();

            page.Response.Output.WriteLine("Connected. Server version: {0}", cn.ServerVersion);
        }
    }
}
