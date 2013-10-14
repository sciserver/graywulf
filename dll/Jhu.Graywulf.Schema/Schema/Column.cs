using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data;

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
            dr[Schema.Constants.SchemaColumnColumnName] = this.Name;
            dr[Schema.Constants.SchemaColumnColumnOrdinal] = this.ID;
            dr[Schema.Constants.SchemaColumnColumnSize] = this.DataType.Size;
            dr[Schema.Constants.SchemaColumnNumericPrecision] = this.DataType.Precision;
            dr[Schema.Constants.SchemaColumnNumericScale] = this.DataType.Scale;
            dr[Schema.Constants.SchemaColumnIsUnique] = this.IsIdentity;    //
            dr[Schema.Constants.SchemaColumnIsKey] = this.isKey;
            dr[Schema.Constants.SchemaColumnDataType] = this.DataType.Type;
            dr[Schema.Constants.SchemaColumnAllowDBNull] = this.IsNullable;
            dr[Schema.Constants.SchemaColumnProviderType] = this.DataType.Name;
            dr[Schema.Constants.SchemaColumnIsAliased] = false; //
            dr[Schema.Constants.SchemaColumnIsExpression] = false; //
            dr[Schema.Constants.SchemaColumnIsIdentity] = this.IsIdentity;
            dr[Schema.Constants.SchemaColumnIsAutoIncrement] = this.IsIdentity;
            dr[Schema.Constants.SchemaColumnIsRowVersion] = false;
            dr[Schema.Constants.SchemaColumnIsHidden] = this.IsHidden;
            dr[Schema.Constants.SchemaColumnIsLong] = this.DataType.IsMax;
            dr[Schema.Constants.SchemaColumnIsReadOnly] = true;
            dr[Schema.Constants.SchemaColumnProviderSpecificDataType] = this.DataType.Name;
        }

        public void CopyFromSchemaTableRow(DataRow dr)
        {
            this.ID = (int)dr[Schema.Constants.SchemaColumnColumnOrdinal];
            this.Name = (string)dr[Schema.Constants.SchemaColumnColumnName];
            this.IsIdentity = dr[Schema.Constants.SchemaColumnIsUnique] == DBNull.Value ? false : (bool)dr[Schema.Constants.SchemaColumnIsUnique];  //
            this.IsKey = dr[Schema.Constants.SchemaColumnIsKey] == DBNull.Value ? false : (bool)dr[Schema.Constants.SchemaColumnIsKey];  //
            this.IsNullable = dr[Schema.Constants.SchemaColumnAllowDBNull] == DBNull.Value ? false : (bool)dr[Schema.Constants.SchemaColumnAllowDBNull];
            this.IsHidden = dr[Schema.Constants.SchemaColumnIsHidden] == DBNull.Value ? false : (bool)dr[Schema.Constants.SchemaColumnIsHidden];

            this.DataType = DataType.Create((Type)dr[Schema.Constants.SchemaColumnDataType]);
        }
    }
}
