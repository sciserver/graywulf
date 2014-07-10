using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private string name;
        private DatasetMetadata metadata;
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
            this.name = null;
            this.metadata = null;
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

        public FileDataReader ExecuteReader()
        {
            return ExecuteReader(CommandBehavior.Default);
        }

        public FileDataReader ExecuteReader(CommandBehavior behavior)
        {
            dataReader = new FileDataReader(file);
            return dataReader;
        }

        ISmartDataReader ISmartCommand.ExecuteReader()
        {
            return this.ExecuteReader();
        }

        ISmartDataReader ISmartCommand.ExecuteReader(CommandBehavior behavior)
        {
            return this.ExecuteReader(behavior);
        }
    }
}
