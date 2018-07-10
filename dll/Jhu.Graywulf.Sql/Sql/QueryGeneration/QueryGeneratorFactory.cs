using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.QueryGeneration
{
    public static class QueryGeneratorFactory
    {
        public static QueryGeneratorBase CreateCodeGenerator(string providerName)
        {
            switch (providerName)
            {
                case Schema.SqlServer.Constants.SqlServerProviderName:
                    return new SqlServer.SqlServerQueryGenerator();
                case Schema.MySql.Constants.MySqlProviderName:
                    return new MySql.MySqlQueryGenerator();
                case Schema.PostgreSql.Constants.PostgreSqlProviderName:
                    return new PostgreSql.PostgreSqlQueryGenerator();
                default:
                    throw new NotImplementedException();
            }
        }

        public static QueryGeneratorBase CreateCodeGenerator(Type t)
        {
            if (t == typeof(Schema.SqlServer.SqlServerDataset) || t.IsSubclassOf(typeof(Schema.SqlServer.SqlServerDataset)))
            {
                return new SqlServer.SqlServerQueryGenerator();
            }
            else if (t == typeof(Schema.MySql.MySqlDataset) || t.IsSubclassOf(typeof(Schema.MySql.MySqlDataset)))
            {
                return new MySql.MySqlQueryGenerator();
            }
            else
            {
                return new PostgreSql.PostgreSqlQueryGenerator();
            }
        }

        public static QueryGeneratorBase CreateCodeGenerator(DatasetBase ds)
        {
            return CreateCodeGenerator(ds.GetType());
        }
    }
}
