using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.IO.Tasks
{
    [Serializable]
    public class TableSourceTable : TableSourceBase, ICloneable
    {
        private Table table;

        public Table Table
        {
            get { return table; }
            set { table = value; }
        }

        public TableSourceTable()
        {
            InitializeMembers();
        }

        public TableSourceTable(TableSourceTable old)
            :base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.table = null;
        }

        private void CopyMembers(TableSourceTable old)
        {
            this.table = Util.DeepCopy.CopyObject(old.table);
        }

        public override object Clone()
        {
            return new TableSourceTable(this);
        }

        internal override IDbConnection OpenConnection()
        {
            var dbf = DbProviderFactories.GetFactory(table.Dataset.ProviderName);
            var cn = dbf.CreateConnection();

            cn.ConnectionString = table.Dataset.ConnectionString;
            cn.Open();

            return cn;
        }

        internal override IDbCommand CreateCommand()
        {
            var dbf = DbProviderFactories.GetFactory(table.Dataset.ProviderName);
            var cmd = dbf.CreateCommand();

            cmd.CommandText = table.GetFullyResolvedName();
            cmd.CommandType = CommandType.TableDirect;

            return cmd;
        }
    }
}
