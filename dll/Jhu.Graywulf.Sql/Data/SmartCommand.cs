﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Data
{
    public class SmartCommand : ISmartCommand
    {
        #region Private member variables

        private DatasetBase dataset;
        private IDbCommand command;
        private BatchProperties properties;
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
            set { command.Connection = value; }
        }

        public IDbTransaction Transaction
        {
            get { return command.Transaction; }
            set { command.Transaction = value; }
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

        public BatchProperties Properties
        {
            get { return properties; }
        }

        public bool RecordsCounted
        {
            get { return recordsCounted; }
            set { recordsCounted = value; }
        }

        #endregion
        #region Constructors and initializers

        public SmartCommand(DatasetBase dataset, IDbCommand command)
        {
            this.dataset = dataset;
            this.command = command;
            this.properties = new BatchProperties();
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
            return new SmartDataReader(dataset, command.ExecuteReader());
        }

        IDataReader IDbCommand.ExecuteReader(CommandBehavior behavior)
        {
            return new SmartDataReader(dataset, command.ExecuteReader(behavior));
        }

        #endregion

        public ISmartDataReader ExecuteReader()
        {
            return new SmartDataReader(dataset, command.ExecuteReader());
        }

        public ISmartDataReader ExecuteReader(CommandBehavior behavior)
        {
            return new SmartDataReader(dataset, command.ExecuteReader(behavior));
        }

        /// <summary>
        /// Wraps the query into a SELECT COUNT(*) FROM (...) query and
        /// the number of records is counted.
        /// </summary>
        private IList<long> CountResults()
        {
            // TODO: this only works with single SELECTs now
            var cg = dataset.CreateCodeGenerator();
            var sql = cg.GenerateCountStarQuery(command.CommandText);

            // Save command text for later
            var original = command.CommandText;

            // Change command to run count query instead
            command.CommandText = sql;

            var recordCounts = new List<long>();

            using (var dr = command.ExecuteReader())
            {
                do
                {
                    dr.Read();
                    recordCounts.Add((dr.GetInt32(0)));
                }
                while (dr.NextResult());
            }

            // Change back command text to original query
            command.CommandText = original;

            return recordCounts;
        }
    }
}
