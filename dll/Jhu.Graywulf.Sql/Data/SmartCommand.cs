using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Data
{
    /// <summary>
    /// Wraps an ordinary DbCommand and implements additional functionality
    /// to count records and query resultset properties.
    /// </summary>
    public class SmartCommand : ISmartCommand
    {
        #region Private member variables

        private DatasetBase dataset;
        private DbCommand command;
        private string name;
        private DatasetMetadata metadata;
        private bool recordsCounted;

        #endregion
        #region IDbCommand properties

        public string CommandText
        {
            get { return command.CommandText; }
            set { command.CommandText = value; }
        }

        public int CommandTimeout
        {
            get { return command.CommandTimeout; }
            set { command.CommandTimeout = value; }
        }

        public CommandType CommandType
        {
            get { return command.CommandType; }
            set { command.CommandType = value; }
        }

        public IDbConnection Connection
        {
            get { return command.Connection; }
            set { command.Connection = (DbConnection)value; }
        }

        public IDbTransaction Transaction
        {
            get { return command.Transaction; }
            set { command.Transaction = (DbTransaction)value; }
        }

        public UpdateRowSource UpdatedRowSource
        {
            get { return command.UpdatedRowSource; }
            set { command.UpdatedRowSource = value; }
        }

        public IDataParameterCollection Parameters
        {
            get { return command.Parameters; }
        }

        #endregion
        #region Properties

        public DatasetBase Dataset
        {
            get { return dataset; }
        }

        public IDbCommand Command
        {
            get { return command; }
        }

        public string Name
        {
            get { return name; }
        }

        public DatasetMetadata Metadata
        {
            get { return metadata; }
        }

        public bool RecordsCounted
        {
            get { return recordsCounted; }
            set { recordsCounted = value; }
        }

        #endregion
        #region Constructors and initializers

        public SmartCommand(DatasetBase dataset, DbCommand command)
        {
            this.dataset = dataset;
            this.command = command;
            this.name = null;
            this.metadata = null;
            this.recordsCounted = false;
        }

        public void Dispose()
        {
            if (command != null)
            {
                command.Dispose();
                command = null;
            }
        }

        #endregion
        #region IDbCommand functions

        public void Prepare()
        {
            command.Prepare();
        }

        public void Cancel()
        {
            command.Cancel();
        }

        public IDbDataParameter CreateParameter()
        {
            return command.CreateParameter();
        }

        public Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            return command.ExecuteNonQueryAsync(cancellationToken);
        }

        public int ExecuteNonQuery()
        {
            return command.ExecuteNonQuery();
        }

        public object ExecuteScalar()
        {
            return command.ExecuteScalar();
        }

        IDataReader IDbCommand.ExecuteReader()
        {
            return ((IDbCommand)this).ExecuteReader(CommandBehavior.Default);
        }

        IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
        {
            return Util.TaskHelper.Wait(ExecuteReaderInternalAsync(behavior, CancellationToken.None));
        }

        #endregion

        public ISmartDataReader ExecuteReader()
        {
            return ExecuteReaderInternal(CommandBehavior.Default);
        }

        public ISmartDataReader ExecuteReader(CommandBehavior behavior)
        {
            return ExecuteReaderInternal(behavior);
        }

        public Task<ISmartDataReader> ExecuteReaderAsync()
        {
            return ExecuteReaderInternalAsync(CommandBehavior.Default, CancellationToken.None);
        }

        public Task<ISmartDataReader> ExecuteReaderAsync(CancellationToken cancellationToken)
        {
            return ExecuteReaderInternalAsync(CommandBehavior.Default, cancellationToken);
        }

        public Task<ISmartDataReader> ExecuteReaderAsync(CommandBehavior behavior)
        {
            return ExecuteReaderInternalAsync(behavior, CancellationToken.None);
        }

        public Task<ISmartDataReader> ExecuteReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            return ExecuteReaderInternalAsync(behavior, cancellationToken);
        }

        private ISmartDataReader ExecuteReaderInternal(CommandBehavior behavior)
        {
            List<long> recordCounts = null;

            // If record counting is on, first execute a query to find the
            // number of records that will be returned.
            if (recordsCounted)
            {
                recordCounts = CountResults();
            }

            // TODO: figure out resultset name and metadata here, then pass it
            // to the data reader for further processing

            var dr = command.ExecuteReader(behavior);
            return new SmartDataReader(dataset, dr, recordCounts);
        }

        private async Task<ISmartDataReader> ExecuteReaderInternalAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            List<long> recordCounts = null;

            // If record counting is on, first execute a query to find the
            // number of records that will be returned.
            if (recordsCounted)
            {
                recordCounts = await CountResultsAsync(cancellationToken);
            }

            // TODO: figure out resultset name and metadata here, then pass it
            // to the data reader for further processing

            // Execute query and wrap into a smart data reader
            var dr = await command.ExecuteReaderAsync(behavior, cancellationToken);
            return new SmartDataReader(dataset, dr, recordCounts);
        }

        private DbCommand CreateCountResultsCommand()
        {
            // TODO: this only works with single SELECTs now
            // and can count only records from query, SPs don't work

            var cg = Sql.CodeGeneration.CodeGeneratorFactory.CreateCodeGenerator(dataset.ProviderName);
            var sql = cg.GenerateCountStarQuery(command.CommandText);

            var res = new List<long>();

            var cmd = command.Connection.CreateCommand();

            // Copy parameters
            for (int i = 0; i < command.Parameters.Count; i++)
            {
                cmd.Parameters.Add(command.Parameters[i]);
            }

            cmd.CommandText = sql;
            cmd.CommandTimeout = command.CommandTimeout;
            cmd.CommandType = command.CommandType;

            cmd.Connection = command.Connection;
            cmd.Transaction = command.Transaction;

            return cmd;
        }

        private List<long> CountResults()
        {
            var res = new List<long>();

            using (var cmd = CreateCountResultsCommand())
            {
                using (var dr = cmd.ExecuteReader())
                {
                    do
                    {
                        dr.Read();
                        res.Add(dr.GetInt64(0));
                    }
                    while (dr.NextResult());
                }
            }

            return res;
        }

        /// <summary>
        /// Wraps the query into a SELECT COUNT(*) FROM (...) query and
        /// the number of records is counted.
        /// </summary>
        private async Task<List<long>> CountResultsAsync(CancellationToken cancellationToken)
        {
            var res = new List<long>();

            using (var cmd = CreateCountResultsCommand())
            {
                using (var dr = await cmd.ExecuteReaderAsync(cancellationToken))
                {
                    do
                    {
                        await dr.ReadAsync(cancellationToken);
                        res.Add(dr.GetInt64(0));
                    }
                    while (await dr.NextResultAsync(cancellationToken));
                }
            }

            return res;
        }
    }
}
