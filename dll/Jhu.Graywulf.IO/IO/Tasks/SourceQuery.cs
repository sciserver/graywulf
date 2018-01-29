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
    /// <summary>
    /// Represents a query, with associated data source and metadata,
    /// that can be used as a source of a data copy operation.
    /// </summary>
    [Serializable]
    [DataContract]
    public class SourceQuery : ICloneable
    {
        #region Private member variables

        private DatasetBase dataset;
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
        public virtual DatasetBase Dataset
        {
            get { return dataset; }
            set { dataset = value; }
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
        public virtual string Query
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

        public SourceQuery()
        {
            InitializeMembers();
        }

        public SourceQuery(SourceQuery old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.dataset = null;
            this.header = null;
            this.query = null;
            this.footer = null;
            this.parameters = new Dictionary<string, object>();
        }

        private void CopyMembers(SourceQuery old)
        {
            this.dataset = Util.DeepCloner.CloneObject(old.dataset);
            this.header = old.header;
            this.query = old.query;
            this.footer = old.footer;
            this.parameters = new Dictionary<string, object>(old.parameters);
        }

        public virtual object Clone()
        {
            return new SourceQuery(this);
        }

        #endregion

        /// <summary>
        /// Opens a connection to the underlying dataset.
        /// </summary>
        /// <returns></returns>
        internal Task<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken)
        {
            return Dataset.OpenConnectionAsync(cancellationToken);
        }

        public async Task<IList<Column>> GetColumnsAsync(CancellationToken cancellationToken)
        {
            using (var cn = await OpenConnectionAsync(cancellationToken))
            {
                using (var cmd = CreateCommand())
                {
                    cmd.Connection = cn;

                    using (var dr = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo, cancellationToken))
                    {
                        return dr.Columns;
                    }
                }
            }
        }
        
        /// <summary>
        /// Creates a database command that can be used to execute the query.
        /// </summary>
        /// <returns></returns>
        internal virtual ISmartCommand CreateCommand()
        {
            var dbf = DbProviderFactories.GetFactory(dataset.ProviderName);
            var cmd = new SmartCommand(dataset, dbf.CreateCommand());

            var sql = new StringBuilder();

            var header = Header;
            if (header != null)
            {
                sql.Append(header);
            }

            var query = Query;
            if (query != null)
            {
                sql.Append(query);
            }

            var footer = Footer;
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

        public virtual TableCopyResult CreateResult()
        {
            return new TableCopyResult();
        }
    }
}
