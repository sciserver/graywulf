using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Data
{
    public interface ISmartDataReader : IDataReader
    {
        string Name { get; }
        long RecordCount { get; }
        DatabaseObjectMetadata Metadata { get; }
        List<Column> Columns { get; }
        List<TypeMapping> TypeMappings { get; }
    }
}
