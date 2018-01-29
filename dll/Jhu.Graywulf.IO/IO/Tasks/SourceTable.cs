using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Data;
namespace Jhu.Graywulf.IO.Tasks
{
    [Serializable]
    [DataContract]
    public class SourceTable : SourceQuery, ICloneable
    {
        #region Private member variables

        private TableOrView table;
        private string orderBy;
        private long from;
        private long max;

        #endregion
        #region Properties

        public override DatasetBase Dataset
        {
            get { return table.Dataset; }
            set { throw new InvalidOperationException(); }
        }

        public override string Query
        {
            get
            {
                var ds = table.Dataset;
                var dbf = DbProviderFactories.GetFactory(ds.ProviderName);
                var cg = Sql.CodeGeneration.CodeGeneratorFactory.CreateCodeGenerator(table.Dataset.ProviderName);
                var sql = cg.GenerateSelectStarQuery(table, orderBy, from, max);

                return sql;
            }
            set { throw new InvalidOperationException(); }
        }

        /// <summary>
        /// Gets or sets the dataset on which this query can be executed.
        /// </summary>
        [DataMember]
        public TableOrView Table
        {
            get { return table; }
            set { table = value; }
        }

        [DataMember]
        public string OrderBy
        {
            get { return orderBy; }
            set { orderBy = value; }
        }

        [DataMember]
        public long From
        {
            get { return from; }
            set { from = value; }
        }

        [DataMember]
        public long Max
        {
            get { return max; }
            set { max = value; }
        }

        #endregion
        #region Constructors and initializers

        public SourceTable()
        {
            InitializeMembers();
        }

        public SourceTable(SourceTable old)
        {
            CopyMembers(old);
        }

        public static SourceTable Create(TableOrView table)
        {
            return Create(table, null, -1, -1);
        }

        public static SourceTable Create(TableOrView table, string orderBy, long from, long max)
        {
            return new SourceTable()
            {
                Table = table,
                OrderBy = orderBy,
                From = from,
                Max = max
            };
        }

        private void InitializeMembers()
        {
            this.table = null;
            this.orderBy = null;
            this.from = -1;
            this.max = -1;
        }

        private void CopyMembers(SourceTable old)
        {
            this.table = (TableOrView)old.table.Clone();
            this.orderBy = old.orderBy;
            this.from = old.from;
            this.max = old.max;
        }

        public override object Clone()
        {
            return new SourceTable(this);
        }

        #endregion
        
        /// <summary>
        /// Creates a database command that can be used to execute the query.
        /// </summary>
        /// <returns></returns>
        internal override ISmartCommand CreateCommand()
        {
            var ds = table.Dataset;
            var dbf = DbProviderFactories.GetFactory(ds.ProviderName);
            var cmd = new SmartCommand(ds, dbf.CreateCommand());
            var sql = Query;

            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
                        
            return cmd;
        }

        public override TableCopyResult CreateResult()
        {
            return new TableCopyResult()
            {
                SourceTable = table
            };
        }
    }
}
