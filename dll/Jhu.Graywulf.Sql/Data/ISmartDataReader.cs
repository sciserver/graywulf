using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Data
{
    public interface ISmartDataReader : IDataReader
    {
        string QueryName { get; set; }
        string ResultsetName { get; set; }
        bool HasRows { get; }
        long RecordCount { get; }
        DatabaseObjectMetadata Metadata { get; }

        // TODO: consider making it into a dictionary
        List<Column> Columns { get; }
        List<TypeMapping> TypeMappings { get; }

        void MatchColumns(IList<Column> columns);
        Task<bool> NextResultAsync();
        Task<bool> NextResultAsync(CancellationToken cancellationToken);
        Task<bool> ReadAsync();
        Task<bool> ReadAsync(CancellationToken cancellationToken);
    }
}
