using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Schema
{
    public interface IDatabaseObject
    {
        DatasetBase Dataset { get; }
        string DatasetName { get; }
        string DatabaseName { get; }
        string SchemaName { get; }
        string ObjectName { get; }
    }
}
