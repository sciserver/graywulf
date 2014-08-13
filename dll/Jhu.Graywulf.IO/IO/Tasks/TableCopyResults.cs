using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.IO.Tasks
{
    [Serializable]
    [CollectionDataContract]
    public class TableCopyResults : List<TableCopyResult>
    {
    }
}
