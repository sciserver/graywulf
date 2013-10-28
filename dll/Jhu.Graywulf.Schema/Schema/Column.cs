using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;
using System.Data.Common;
using Jhu.Graywulf.Types;

namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Reflects a table or view column
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class Column : Variable, ICloneable
    {
        private bool isNullable;
        private bool isIdentity;
        private bool isKey;
        private bool isHidden;

        public string ColumnName
        {
            get { return Name; }
            set { Name = value; }
        }

        public bool IsNullable
        {
            get { return isNullable; }
            set { isNullable = value; }
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

        /// <summary>
        /// Constructor that initializes the parent table or view
        /// </summary>
        /// <param name="parent"></param>
        public Column(DatabaseObject parent)
            :base(parent)
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

        /// <summary>
        /// Initializes member variables
        /// </summary>
        private void InitializeMembers()
        {
            this.isNullable = false;
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
            this.isNullable = old.isNullable;
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

        public void CopyToSchemaTableRow(DataRow dr)
        {
            dr[SchemaTableColumn.ColumnName] = this.Name;
            dr[SchemaTableColumn.ColumnOrdinal] = this.ID;
            dr[SchemaTableColumn.ColumnSize] = this.DataType.Size;
            dr[SchemaTableColumn.NumericPrecision] = this.DataType.Precision;
            dr[SchemaTableColumn.NumericScale] = this.DataType.Scale;
            dr[SchemaTableColumn.IsUnique] = this.IsIdentity;    //
            dr[SchemaTableColumn.IsKey] = this.isKey;
            dr[SchemaTableColumn.DataType] = this.DataType.Type;
            dr[SchemaTableColumn.AllowDBNull] = this.IsNullable;
            dr[SchemaTableColumn.ProviderType] = this.DataType.Name;
            dr[SchemaTableColumn.IsAliased] = false; //
            dr[SchemaTableColumn.IsExpression] = false; //
            //dr[SchemaTableOptionalColumn.IsIdentity] = this.IsIdentity;
            dr[SchemaTableOptionalColumn.IsAutoIncrement] = this.IsIdentity;
            dr[SchemaTableOptionalColumn.IsRowVersion] = false;
            dr[SchemaTableOptionalColumn.IsHidden] = this.IsHidden;
            dr[SchemaTableColumn.IsLong] = this.DataType.IsMax;
            dr[SchemaTableOptionalColumn.IsReadOnly] = true;
            dr[SchemaTableOptionalColumn.ProviderSpecificDataType] = this.DataType.Name;

            
        }

        public void CopyFromSchemaTableRow(DataRow dr)
        {
            this.ID = (int)dr[SchemaTableColumn.ColumnOrdinal];
            this.Name = (string)dr[SchemaTableColumn.ColumnName];
            this.isIdentity = dr[SchemaTableColumn.IsUnique] == DBNull.Value ? false : (bool)dr[SchemaTableColumn.IsUnique];  //
            this.isKey = dr[SchemaTableColumn.IsKey] == DBNull.Value ? false : (bool)dr[SchemaTableColumn.IsKey];  //
            this.isNullable = dr[SchemaTableColumn.AllowDBNull] == DBNull.Value ? false : (bool)dr[SchemaTableColumn.AllowDBNull];
            this.isHidden = dr[SchemaTableOptionalColumn.IsHidden] == DBNull.Value ? false : (bool)dr[SchemaTableOptionalColumn.IsHidden];

            this.DataType = DataType.Create(dr);
        }
    }
}
