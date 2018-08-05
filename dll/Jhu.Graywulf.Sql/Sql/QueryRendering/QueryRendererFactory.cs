using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.QueryRendering;

namespace Jhu.Graywulf.Sql.QueryRendering
{
    public static class QueryRendererFactory
    {
        public static QueryRendererBase CreateQueryRenderer(string providerName)
        {
            switch (providerName)
            {
                case Schema.SqlServer.Constants.SqlServerProviderName:
                    return new SqlServer.SqlServerQueryRenderer();
                case Schema.MySql.Constants.MySqlProviderName:
                    return new MySql.MySqlQueryRenderer();
                case Schema.PostgreSql.Constants.PostgreSqlProviderName:
                    return new PostgreSql.PostgreSqlQueryRenderer();
                default:
                    throw new NotImplementedException();
            }
        }

        public static QueryRendererBase CreateQueryRenderer(Type t)
        {
            if (t == typeof(Schema.SqlServer.SqlServerDataset) || t.IsSubclassOf(typeof(Schema.SqlServer.SqlServerDataset)))
            {
                return new SqlServer.SqlServerQueryRenderer();
            }
            else if (t == typeof(Schema.MySql.MySqlDataset) || t.IsSubclassOf(typeof(Schema.MySql.MySqlDataset)))
            {
                return new MySql.MySqlQueryRenderer();
            }
            else
            {
                return new PostgreSql.PostgreSqlQueryRenderer();
            }
        }

        public static QueryRendererBase CreateQueryRenderer(DatasetBase ds)
        {
            return CreateQueryRenderer(ds.GetType());
        }
    }
}
