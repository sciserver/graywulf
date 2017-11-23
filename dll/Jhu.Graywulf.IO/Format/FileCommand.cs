using System;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Data;

namespace Jhu.Graywulf.Format
{
    public class FileCommand : ISmartCommand, IDisposable
    {
        #region Private member variables

        private DataFileBase file;
        private FileDataReader dataReader;
        private bool recordsCounted;

        #endregion
        #region IDbCommand properties

        public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public CommandType CommandType { get; set; }

        public IDbConnection Connection { get; set; }

        public IDbTransaction Transaction { get; set; }

        public UpdateRowSource UpdatedRowSource { get; set; }

        public IDataParameterCollection Parameters
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
        #region Properties

        public DataFileBase File
        {
            get { return file; }
            set { file = value; }
        }

        public string Name
        {
            get { return file.Name; }
        }

        public DatasetMetadata Metadata
        {
            get { return file.Metadata; }
        }

        public bool RecordsCounted
        {
            get { return recordsCounted; }
            set { recordsCounted = value; }
        }

        #endregion
        #region Constructors and initializers

        public FileCommand()
        {
            InitializeMembers();
        }

        public FileCommand(DataFileBase file)
        {
            InitializeMembers();

            this.file = file;
        }

        private void InitializeMembers()
        {
            this.file = null;
            this.dataReader = null;
            this.recordsCounted = false;
        }

        public void Dispose()
        {
            if (dataReader != null)
            {
                dataReader.Dispose();
                dataReader = null;
            }
        }

        #endregion
        #region IDbCommand functions

        public void Prepare()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            // TODO: send cancel signal to the data reader to
            // mimic behaviour of database commands
            // dataReader.cancel
        }

        public IDbDataParameter CreateParameter()
        {
            throw new NotImplementedException();
        }

        public int ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public object ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        IDataReader IDbCommand.ExecuteReader()
        {
            return ExecuteReader(CommandBehavior.Default);
        }

        IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
        {
            return ExecuteReader(behavior);
        }

        #endregion

        ISmartDataReader ISmartCommand.ExecuteReader()
        {
            return ExecuteReader();
        }

        ISmartDataReader ISmartCommand.ExecuteReader(CommandBehavior behavior)
        {
            return ExecuteReader(behavior);
        }

        public FileDataReader ExecuteReader()
        {
            return ExecuteReader(CommandBehavior.Default);
        }

        public FileDataReader ExecuteReader(CommandBehavior behavior)
        {
            dataReader = new FileDataReader(file);
            return dataReader;
        }

        public Task<ISmartDataReader> ExecuteReaderAsync()
        {
            return ExecuteReaderAsync(CommandBehavior.Default, CancellationToken.None);
        }

        public Task<ISmartDataReader> ExecuteReaderAsync(CommandBehavior behavior)
        {
            return ExecuteReaderAsync(behavior, CancellationToken.None);
        }

        public Task<ISmartDataReader> ExecuteReaderAsync(CancellationToken cancellationToken)
        {
            return ExecuteReaderAsync(CommandBehavior.Default, cancellationToken);
        }

        public Task<ISmartDataReader> ExecuteReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            // TODO make it async
            // TODO forward cancellation token
            dataReader = new FileDataReader(file);
            return Task.FromResult((ISmartDataReader)dataReader);
        }
        
        async Task<ISmartDataReader> ISmartCommand.ExecuteReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            return await this.ExecuteReaderAsync(behavior, cancellationToken);
        }
    }
}
