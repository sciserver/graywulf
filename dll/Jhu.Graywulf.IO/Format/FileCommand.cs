using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Jhu.Graywulf.Format
{
    public class FileCommand : IDbCommand, IDisposable
    {
        private DataFileBase file;
        private FileDataReader dataReader;

        public DataFileBase File
        {
            get { return file; }
            set { file = value; }
        }

        public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public CommandType CommandType { get; set; }

        public IDbConnection Connection { get; set; }

        public IDbTransaction Transaction { get; set; }

        public UpdateRowSource UpdatedRowSource { get; set; }

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

        public FileDataReader ExecuteReader()
        {
            return ExecuteReader(CommandBehavior.Default);
        }

        public FileDataReader ExecuteReader(CommandBehavior behavior)
        {
            dataReader = new FileDataReader(file);
            return dataReader;
        }

        IDataReader IDbCommand.ExecuteReader()
        {
            return ExecuteReader(CommandBehavior.Default);
        }

        IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
        {
            return ExecuteReader(behavior);
        }

        public object ExecuteScalar()
        {
            throw new NotImplementedException();
        }

        public IDataParameterCollection Parameters
        {
            get { throw new NotImplementedException(); }
        }

        public void Prepare()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (dataReader != null)
            {
                dataReader.Dispose();
                dataReader = null;
            }
        }
    }
}
