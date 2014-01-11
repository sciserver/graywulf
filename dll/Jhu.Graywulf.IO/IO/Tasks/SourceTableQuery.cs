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
    public class SourceTableQuery : ICloneable
    {
        private DatasetBase dataset;
        private string query;

        public DatasetBase Dataset
        {
            get { return dataset; }
            set { dataset = value; }
        }

        public string Query
        {
            get { return query; }
            set { query = value; }
        }

        public SourceTableQuery()
        {
            InitializeMembers();
        }

        public SourceTableQuery(SourceTableQuery old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.dataset = null;
            this.query = null;
        }

        private void CopyMembers(SourceTableQuery old)
        {
            this.dataset = Util.DeepCloner.CloneObject(old.dataset);
            this.query = old.query;
        }

        public object Clone()
        {
            return new SourceTableQuery(this);
        }

        internal IDbConnection OpenConnection()
        {
            var dbf = DbProviderFactories.GetFactory(dataset.ProviderName);
            var cn = dbf.CreateConnection();

            cn.ConnectionString = dataset.ConnectionString;
            cn.Open();

            return cn;
        }

        internal IDbCommand CreateCommand()
        {
            var dbf = DbProviderFactories.GetFactory(dataset.ProviderName);
            var cmd = dbf.CreateCommand();

            cmd.CommandText = query;
            cmd.CommandType = CommandType.Text;

            return cmd;
        }

        public DataTable GetSchemaTable()
        {
            using (var cn = OpenConnection())
            {
                using (var cmd = cn.CreateCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandText = query;

                    using (var dr = cmd.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
                    {
                        return dr.GetSchemaTable();
                    }
                }
            }
        }
    }
}
