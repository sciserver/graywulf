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

        // TODO: consider making it into a dictionary
        List<Column> Columns { get; }
        List<TypeMapping> TypeMappings { get; }
    }
}
