using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        Task<bool> NextResultAsync();
        Task<bool> NextResultAsync(CancellationToken cancellationToken);
        Task<bool> ReadAsync();
        Task<bool> ReadAsync(CancellationToken cancellationToken);
    }
}
