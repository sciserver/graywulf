using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Data;

namespace Jhu.Graywulf.IO.Tasks
{
    /// <summary>
    /// Represents a query, with associated data source and metadata,
    /// that can be used as a source of a data copy operation.
    /// </summary>
    [Serializable]
    [DataContract]
    public class SourceTableQuery : ICloneable
    {
        #region Private member variables

        private DatasetBase dataset;
        private string sourceSchemaName;
        private string sourceObjectName;
        private string header;
        private string query;
        private string footer;
        private Dictionary<string, object> parameters;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the dataset on which this query can be executed.
        /// </summary>
        [DataMember]
        public DatasetBase Dataset
        {
            get { return dataset; }
            set { dataset = value; }
        }

        /// <summary>
        /// Gets or sets the schema name of the table or view from
        /// which data is to be returned.
        /// </summary>
        [DataMember]
        public string SourceSchemaName
        {
            get { return sourceSchemaName; }
            set { sourceSchemaName = value; }
        }

        /// <summary>
        /// Gets or sets the name table from which data is returned.
        /// </summary>
        [DataMember]
        public string SourceObjectName
        {
            get { return sourceObjectName; }
            set { sourceObjectName = value; }
        }

        /// <summary>
        /// Gets or sets the query script header.
        /// </summary>
        [DataMember]
        public string Header
        {
            get { return header; }
            set { header = value; }
        }

        /// <summary>
        /// Gets or sets the text of the query.
        /// </summary>
        [DataMember]
        public string Query
        {
            get { return query; }
            set { query = value; }
        }

        /// <summary>
        /// Gets or sets the query script footer.
        /// </summary>
        [DataMember]
        public string Footer
        {
            get { return footer; }
            set { footer = value; }
        }

        [DataMember]
        public Dictionary<string, object> Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        #endregion
        #region Constructors and initializers

        public SourceTableQuery()
        {
            InitializeMembers();
        }

        public SourceTableQuery(SourceTableQuery old)
        {
            CopyMembers(old);
        }

        public static SourceTableQuery Create(TableOrView table)
        {
            return Create(table, Int32.MaxValue);
        }

        public static SourceTableQuery Create(TableOrView table, int top)
        {
            var cg = SqlCodeGen.CodeGeneratorFactory.CreateCodeGenerator(table.Dataset.ProviderName);

            return new SourceTableQuery()
            {
                Dataset = table.Dataset,
                SourceSchemaName = table.SchemaName,
                SourceObjectName = table.ObjectName,
                Query = cg.GenerateSelectStarQuery(table, top)
            };
        }

        private void InitializeMembers()
        {
            this.dataset = null;
            this.sourceSchemaName = null;
            this.sourceObjectName = null;
            this.header = null;
            this.query = null;
            this.footer = null;
            this.parameters = new Dictionary<string, object>();
        }

        private void CopyMembers(SourceTableQuery old)
        {
            this.dataset = Util.DeepCloner.CloneObject(old.dataset);
            this.sourceSchemaName = old.sourceSchemaName;
            this.sourceObjectName = old.sourceObjectName;
            this.header = old.header;
            this.query = old.query;
            this.footer = old.footer;
            this.parameters = new Dictionary<string, object>(old.parameters);
        }

        public object Clone()
        {
            return new SourceTableQuery(this);
        }

        #endregion

        /// <summary>
        /// Opens a connection to the underlying dataset.
        /// </summary>
        /// <returns></returns>
        internal IDbConnection OpenConnection()
        {
            return dataset.OpenConnection();
        }

        /// <summary>
        /// Creates a database command that can be used to execute the query.
        /// </summary>
        /// <returns></returns>
        internal ISmartCommand CreateCommand()
        {
            var dbf = DbProviderFactories.GetFactory(dataset.ProviderName);
            var cmd = new SmartCommand(dataset, dbf.CreateCommand());

            var sql = new StringBuilder();

            if (header != null)
            {
                sql.Append(header);
            }

            if (query != null)
            {
                sql.Append(query);
            }

            if (footer != null)
            {
                sql.Append(footer);
            }

            cmd.CommandText = sql.ToString();
            cmd.CommandType = CommandType.Text;

            if (parameters != null)
            {
                foreach (string name in parameters.Keys)
                {
                    var par = cmd.CreateParameter();
                    par.ParameterName = name;
                    par.Value = parameters[name];
                    cmd.Parameters.Add(par);
                }
            }

            return cmd;
        }

        public IList<Column> GetColumns()
        {
            using (var cn = OpenConnection())
            {
                using (var cmd = CreateCommand())
                {
                    cmd.Connection = cn;

                    using (var dr = cmd.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        return dr.Columns;
                    }
                }
            }
        }
    }
}
