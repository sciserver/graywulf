using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Extensions.NameResolution
{
    public abstract class GraywulfSqlNameResolverTestBase : Parsing.ParsingTestBase
    {
        protected override SqlNameResolver CreateNameResolver()
        {
            return new GraywulfSqlNameResolver()
            {
                SchemaManager = SchemaManager,
                Options = new GraywulfSqlNameResolverOptions()
                {
                    DefaultTableDatasetName = Jhu.Graywulf.Test.Constants.TestDatasetName,
                    DefaultDataTypeDatasetName = Jhu.Graywulf.Test.Constants.TestDatasetName,
                    DefaultFunctionDatasetName = Jhu.Graywulf.Test.Constants.TestDatasetName,
                    DefaultOutputDatasetName = Jhu.Graywulf.Test.Constants.TestDatasetName,
                }
            };
        }
    }
}
