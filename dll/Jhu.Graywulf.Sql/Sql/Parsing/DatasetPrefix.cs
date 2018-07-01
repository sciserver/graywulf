using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class DatasetPrefix
    {
        public string DatasetName
        {
            get { return FindDescendant<DatasetName>()?.Value; }
        }

        public static DatasetPrefix Create(string datasetName)
        {
            var ndn = new DatasetName();
            ndn.Stack.AddLast(Identifier.Create(datasetName));

            var ndp = new DatasetPrefix();
            ndp.Stack.AddLast(ndn);
            ndp.Stack.AddLast(Colon.Create());

            return ndp;
        }
    }
}
