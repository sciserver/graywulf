using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Check
{
    public class SqlServerCheck : CheckRoutineBase
    {
        public override CheckCategory Category
        {
            get
            {
                return CheckCategory.Service;
            }
        }

        public string ConnectionString { get; set; }

        public SqlServerCheck(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public override void Execute(TextWriter output)
        {
            var csb = new SqlConnectionStringBuilder(ConnectionString);

            output.WriteLine(
                "Testing connection to server: {0}",
                csb.DataSource);

            var cn = new SqlConnection(ConnectionString);
            cn.Open();

            output.WriteLine("Connected. Server version: {0}", cn.ServerVersion);
        }
    }
}
