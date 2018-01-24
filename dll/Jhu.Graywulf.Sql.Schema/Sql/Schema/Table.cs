using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Sql.Schema
{
    /// <summary>
    /// Reflects information about a database table
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class Table : TableOrView, ICloneable
    {
        /// <summary>
        /// Gets or sets the name of the table
        /// </summary>
        [IgnoreDataMember]
        public string TableName
        {
            get { return ObjectName; }
            set { ObjectName = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Table()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Creates a table and initializes its dataset
        /// </summary>
        /// <param name="dataset"></param>
        public Table(DatasetBase dataset)
            : base(dataset)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public Table(Table old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variables to their default values
        /// </summary>
        private void InitializeMembers()
        {
            this.ObjectType = DatabaseObjectType.Table;
        }

        /// <summary>
        /// Copies member variables
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(Table old)
        {
            this.ObjectType = old.ObjectType;
        }

        /// <summary>
        /// Returns a copy of this table
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new Table(this);
        }

        private void CopyColumnsFrom(IList<Column> columns)
        {
            this.Columns = new ConcurrentDictionary<string, Column>(SchemaManager.Comparer);

            for (int i = 0; i < columns.Count; i++)
            {
                var nc = (Column)columns[i].Clone();
                this.Columns.TryAdd(nc.Name, nc);
            }

            if (PrimaryKey != null)
            {

            }
        }

        private void CreatePrimaryKeyFrom(IList<Column> columns)
        {
            Index pk;

            if (this.PrimaryKey != null)
            {
                this.Indexes.TryRemove(this.PrimaryKey.IndexName, out pk);
            }

            var pkcolumns = columns.Where(c => c.IsKey).ToArray();

            if (pkcolumns.Length > 0)
            {
                pk = new Index(this, pkcolumns, null, true)
                {
                    
                };

                this.Indexes.TryAdd(pk.IndexName, pk);
            }
        }

        /// <summary>
        /// Creates the table
        /// </summary>
        public override void Create()
        {
            Create(true, true);
        }

        /// <summary>
        /// Creates the table
        /// </summary>
        public void Create(bool createPrimaryKey, bool createIndexes)
        {
            Dataset.CreateTable(this, createPrimaryKey, createIndexes);
        }

        /// <summary>
        /// Truncates the table
        /// </summary>
        public void Truncate()
        {
            Dataset.TruncateTable(this);
        }

        /// <summary>
        /// Verify the table schema by comparing columns to those are
        /// actually present in the database.
        /// </summary>
        /// <returns></returns>
        public bool VerifyColumns(bool observeColumnOrder)
        {
            var tempcols = LoadColumns();

            foreach (var key in tempcols.Keys)
            {
                if (!this.Columns.ContainsKey(key))
                {
                    return false;
                }

                if (!this.Columns[key].Compare(tempcols[key], observeColumnOrder))
                {
                    return false;
                }
            }

            return true;
        }

        #region Specialized operations

        public void Initialize(IList<Column> columns, TableInitializationOptions options)
        {
            // If the table needs to be dropped do it now
            if ((options & TableInitializationOptions.Drop) != 0)
            {
                Drop();
            }

            // If the destination table is supposed to be existing take columns
            // from there, otherwise take from input data reader
            if ((options & TableInitializationOptions.Append) == 0 &&
                (options & TableInitializationOptions.Create) == 0)
            {
                // Destination table columns are already loaded
            }
            else
            {
                CopyColumnsFrom(columns);
                CreatePrimaryKeyFrom(columns);

                if ((options & TableInitializationOptions.Append) != 0)
                {
                    if (!VerifyColumns(true))
                    {
                        throw new SchemaException(
                            String.Format(
                                "Table is to be appended but schemas do not match: {0}:{1}.{2}.{3}",
                                Dataset.Name, DatabaseName, SchemaName, TableName));  // *** TODO
                    }
                }
                else if ((options & TableInitializationOptions.Create) != 0)
                {
                    Create
                        (options.HasFlag(TableInitializationOptions.CreatePrimaryKey),
                        (options & TableInitializationOptions.CreateIndexes) != 0);
                }
                else
                {
                    // *** TODO: implement other options
                    throw new NotImplementedException();
                }
            }

            if ((options & TableInitializationOptions.Clear) != 0)
            {
                Truncate();
            }
        }

        #endregion
    }
}
