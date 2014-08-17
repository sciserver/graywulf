using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;

namespace Jhu.Graywulf.SqlCodeGen
{
    public static class CodeGeneratorFactory
    {
        public static SqlCodeGeneratorBase CreateCodeGenerator(string providerName)
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
    }
}
