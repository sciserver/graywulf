using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Sql.CodeGeneration
{
    public class SqlCodeGeneratorFactory
    {
        public static SqlCodeGeneratorBase CreateCodeGenerator(Type t)
        {
            if (t == typeof(Schema.SqlServer.SqlServerDataset) || t.IsSubclassOf(typeof(Schema.SqlServer.SqlServerDataset)))
            {
                return new SqlServer.SqlServerCodeGenerator();
            }
            else if (t == typeof(Schema.MySql.MySqlDataset) || t.IsSubclassOf(typeof(Schema.MySql.MySqlDataset)))
            {
                return new MySql.MySqlCodeGenerator();
            }
            else
            {
                return new PostgreSql.PostgreSqlCodeGenerator();
            }
        }

        public static SqlCodeGeneratorBase CreateCodeGenerator(DatasetBase ds)
        {
            return CreateCodeGenerator(ds.GetType());
        }
    }
}
