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

        protected override IEnumerable<CheckRoutineStatus> OnExecute()
        {
            var csb = new SqlConnectionStringBuilder(ConnectionString);

            yield return ReportInfo("Testing connection to SQL server: {0}", csb.DataSource);

            var cn = new SqlConnection(ConnectionString);
            cn.Open();

            yield return ReportSuccess("Connected. Server version: {0}", cn.ServerVersion);
        }
    }
}
