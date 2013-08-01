using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Metadata
{
    public class Installer
    {
        public Installer()
        {
        }

        public void CreateSchema(string connectionString)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();

                string[] scripts = SplitSqlScript(Scripts.Jhu_Graywulf_Metadata);

                foreach (string sql in scripts)
                {
                    if (!sql.StartsWith("USE"))
                    {
                        using (SqlCommand cmd = new SqlCommand(sql, cn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public string[] SplitSqlScript(string script)
        {
            return script.Split(new string[] { "\r\nGO", "\nGO" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
