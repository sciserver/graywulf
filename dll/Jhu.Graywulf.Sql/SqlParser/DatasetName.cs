using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SqlParser
{
    public partial class DatasetName
    {
        public static DatasetName Create(string datasetName)
        {
            var ntn = new DatasetName();
            ntn.Stack.AddLast(Identifier.Create(datasetName));

            return ntn;
        }
    }
}
