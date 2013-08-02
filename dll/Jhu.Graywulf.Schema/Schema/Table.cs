using System;
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
    }
}
