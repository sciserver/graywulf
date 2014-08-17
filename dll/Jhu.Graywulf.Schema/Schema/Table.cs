using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Schema
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
        /// Gets the primary key of the table
        /// </summary>
        [IgnoreDataMember]
        public Index PrimaryKey
        {
            get { return Indexes.Values.FirstOrDefault(i => i.IsPrimaryKey); }
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

        public void Initialize(IList<Column> columns, TableInitializationOptions options)
        {
            if ((options & TableInitializationOptions.Drop) != 0)
            {
                Drop();
            }

            // Copy columns
            Columns = new ConcurrentDictionary<string, Column>(SchemaManager.Comparer);
            for (int i = 0; i < columns.Count; i++)
            {
                var nc = (Column)columns[i].Clone();
                nc.ID = i;
                Columns.TryAdd(columns[i].Name, nc);
            }

            if ((options & TableInitializationOptions.Append) != 0)
            {
                if (!VerifyColumns(true))
                {
                    throw new SchemaException("Table is to be appended but schemas do not match.");  // *** TODO
                }
            }
            else if ((options & TableInitializationOptions.Create) != 0)
            {
                Create();
            }
            else
            {
                // *** TODO: implement other options
                throw new NotImplementedException();
            }

            if ((options & TableInitializationOptions.Clear) != 0)
            {
                Truncate();
            }
        }

        /// <summary>
        /// Creates the table
        /// </summary>
        public void Create()
        {
            Dataset.CreateTable(this);
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
    }
}
