using System;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Data
{
    public interface ISmartCommand : IDbCommand
    {
        string Name { get; }
        DatasetMetadata Metadata { get; }

        bool RecordsCounted { get; set; }
        ISmartDataReader ExecuteReader();
        ISmartDataReader ExecuteReader(CommandBehavior behavior);

        Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken);
        Task<ISmartDataReader> ExecuteReaderAsync();
        Task<ISmartDataReader> ExecuteReaderAsync(CommandBehavior behavior);
        Task<ISmartDataReader> ExecuteReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken);
        Task<ISmartDataReader> ExecuteReaderAsync(CancellationToken cancellationToken);
    }
}
