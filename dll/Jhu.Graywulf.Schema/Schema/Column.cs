using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Data.Common;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Reflects a table or view column
    /// </summary>
    [Serializable]
    public class Column : Variable, ICloneable
    {
        private bool isIdentity;
        private bool isKey;
        private bool isHidden;

        public string ColumnName
        {
            get { return Name; }
            set { Name = value; }
        }

        public bool IsIdentity
        {
            get { return isIdentity; }
            set { isIdentity = value; }
        }

        public bool IsKey
        {
            get { return isKey; }
            set { isKey = value; }
        }

        public bool IsHidden
        {
            get { return isHidden; }
            set { isHidden = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Column()
        {
            InitializeMembers();
        }

        public Column(string name, DataType type)
        {
            InitializeMembers();

            this.Name = name;
            this.DataType = type;
        }

        public Column(string name, Type type, short size)
        {
            InitializeMembers();

            this.Name = name;
            this.DataType = DataType.Create(type, size);
        }

        /// <summary>
        /// Constructor that initializes the parent table or view
        /// </summary>
        /// <param name="parent"></param>
        public Column(DatabaseObject parent)
            : base(parent)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public Column(Column old)
        {
            CopyMembers(old);
        }

        public static Column Create(DataRow dr)
        {
            var column = new Column();
            column.CopyFromSchemaTableRow(dr);

            return column;
        }

        /// <summary>
        /// Initializes member variables
        /// </summary>
        private void InitializeMembers()
        {
            this.isIdentity = false;
            this.isKey = false;
            this.isHidden = false;
        }

        /// <summary>
        /// Copies member variables
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(Column old)
        {
            this.isIdentity = old.isIdentity;
            this.isKey = old.isKey;
            this.isHidden = old.isHidden;
        }

        /// <summary>
        /// Returns a copy of the column
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return new Column(this);
        }

        public bool Compare(Column other)
        {
            var res = true;

            res &= this.ID == other.ID;
            res &= SchemaManager.Comparer.Compare(this.Name, other.Name) == 0;
            res &= this.DataType.Compare(other.DataType);

            return res;
        }

        public void CopyToSchemaTableRow(DataRow dr)
        {
            dr[SchemaTableColumn.ColumnName] = this.Name;
            dr[SchemaTableColumn.ColumnOrdinal] = this.ID;
            dr[SchemaTableColumn.ColumnSize] = this.DataType.Length;
            dr[SchemaTableColumn.NumericPrecision] = this.DataType.Precision;
            dr[SchemaTableColumn.NumericScale] = this.DataType.Scale;
            dr[SchemaTableColumn.IsUnique] = this.IsIdentity;    //
            dr[SchemaTableColumn.IsKey] = this.isKey;
            dr[SchemaTableColumn.DataType] = this.DataType.Type;
            dr[SchemaTableColumn.AllowDBNull] = this.DataType.IsNullable;
            dr[SchemaTableColumn.ProviderType] = this.DataType.Name;
            dr[SchemaTableColumn.IsAliased] = false; //
            dr[SchemaTableColumn.IsExpression] = false; //
            //dr[SchemaTableOptionalColumn.IsIdentity] = this.IsIdentity;
            dr[SchemaTableOptionalColumn.IsAutoIncrement] = this.IsIdentity;
            dr[SchemaTableOptionalColumn.IsRowVersion] = false;
            dr[SchemaTableOptionalColumn.IsHidden] = this.IsHidden;
            dr[SchemaTableColumn.IsLong] = this.DataType.IsMaxLength;
            dr[SchemaTableOptionalColumn.IsReadOnly] = true;
            dr[SchemaTableOptionalColumn.ProviderSpecificDataType] = this.DataType.Name;
        }

        public void CopyFromSchemaTableRow(DataRow dr)
        {
            this.ID = (int)dr[SchemaTableColumn.ColumnOrdinal];
            this.Name = (string)dr[SchemaTableColumn.ColumnName];
            this.IsIdentity = dr[SchemaTableColumn.IsUnique] == DBNull.Value ? false : (bool)dr[SchemaTableColumn.IsUnique];  //
            this.IsKey = dr[SchemaTableColumn.IsKey] == DBNull.Value ? false : (bool)dr[SchemaTableColumn.IsKey];  //
            this.IsHidden = dr[SchemaTableOptionalColumn.IsHidden] == DBNull.Value ? false : (bool)dr[SchemaTableOptionalColumn.IsHidden];

            this.DataType = DataType.Create(dr);
        }
    }
}
