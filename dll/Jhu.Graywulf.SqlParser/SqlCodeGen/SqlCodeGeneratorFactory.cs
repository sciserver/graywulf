using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.SqlParser.SqlCodeGen;

namespace Jhu.Graywulf.SqlCodeGen
{
    public class SqlCodeGeneratorFactory
    {
        public static SqlCodeGeneratorBase CreateCodeGenerator(Type t)
        {
            if (t == typeof(Schema.SqlServer.SqlServerDataset) || t.IsSubclassOf(typeof(Schema.SqlServer.SqlServerDataset)))
            {
                return new SqlServerCodeGenerator();
            }
            else if (t == typeof(Schema.MySql.MySqlDataset) || t.IsSubclassOf(typeof(Schema.MySql.MySqlDataset)))
            {
                return new MySqlCodeGenerator();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static SqlCodeGeneratorBase CreateCodeGenerator(DatasetBase ds)
        {
            return CreateCodeGenerator(ds.GetType());
        }
    }
}
