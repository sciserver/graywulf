using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.CodeGeneration
{
    public static class CodeGeneratorFactory
    {
        public static CodeGeneratorBase CreateCodeGenerator(string providerName)
        {
            switch (providerName)
            {
                case Schema.SqlServer.Constants.SqlServerProviderName:
                    return new SqlServer.SqlServerCodeGenerator();
                case Schema.MySql.Constants.MySqlProviderName:
                    return new MySql.MySqlCodeGenerator();
                case Schema.PostgreSql.Constants.PostgreSqlProviderName:
                    return new PostgreSql.PostgreSqlCodeGenerator();
                default:
                    throw new NotImplementedException();
            }
        }

        public static CodeGeneratorBase CreateCodeGenerator(Type t)
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

        public static CodeGeneratorBase CreateCodeGenerator(DatasetBase ds)
        {
            return CreateCodeGenerator(ds.GetType());
        }
    }
}
