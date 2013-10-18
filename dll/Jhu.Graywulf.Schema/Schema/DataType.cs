using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;

namespace Jhu.Graywulf.Schema
{
    [Serializable]
    public class DataType : ICloneable
    {
        public static DataType Unknown
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameUnknown,
                    Type = null,
                    SqlDbType = SqlDbType.Int,
                    Scale = 0,
                    Precision = 0,
                    Size = 0,
                    HasSize = false,
                    MaxSize = 0,
                    VarSize = false,
                };
            }
        }

        public static DataType TinyInt
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameTinyInt,
                    Type = typeof(SByte),
                    SqlDbType = SqlDbType.TinyInt,
                    Scale = 0,
                    Precision = 3,
                    Size = 1,
                    HasSize = false,
                    MaxSize = 1,
                    VarSize = false,
                };
            }
        }

        public static DataType SmallInt
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameSmallInt,
                    Type = typeof(Int16),
                    SqlDbType = SqlDbType.SmallInt,
                    Scale = 0,
                    Precision = 5,
                    Size = 2,
                    HasSize = false,
                    MaxSize = 2,
                    VarSize = false,
                };
            }
        }

        public static DataType Int
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameInt,
                    Type = typeof(Int32),
                    SqlDbType = SqlDbType.Int,
                    Scale = 0,
                    Precision = 10,
                    Size = 4,
                    HasSize = false,
                    MaxSize = 4,
                    VarSize = false,
                };
            }
        }

        public static DataType BigInt
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameBigInt,
                    Type = typeof(Int64),
                    SqlDbType = SqlDbType.BigInt,
                    Scale = 0,
                    Precision = 19,
                    Size = 8,
                    HasSize = false,
                    MaxSize = 8,
                    VarSize = false,
                };
            }
        }

        public static DataType Bit
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameBit,
                    Type = typeof(Boolean),
                    SqlDbType = SqlDbType.Bit,
                    Scale = 0,
                    Precision = 1,
                    Size = 1,
                    HasSize = false,
                    MaxSize = 1,
                    VarSize = false,
                };
            }
        }

        public static DataType Decimal
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameDecimal,
                    Type = typeof(Decimal),
                    SqlDbType = SqlDbType.Decimal,
                    Scale = 38,
                    Precision = 38,
                    Size = 17,
                    HasSize = false,
                    MaxSize = 17,
                    VarSize = false,
                };
            }
        }

        public static DataType SmallMoney
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameSmallMoney,
                    Type = typeof(Decimal),
                    SqlDbType = SqlDbType.SmallMoney,
                    Scale = 4,
                    Precision = 10,
                    Size = 4,
                    HasSize = false,
                    MaxSize = 4,
                    VarSize = false,
                };
            }
        }

        public static DataType Money
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameMoney,
                    Type = typeof(Decimal),
                    SqlDbType = SqlDbType.Money,
                    Scale = 4,
                    Precision = 19,
                    Size = 8,
                    HasSize = false,
                    MaxSize = 8,
                    VarSize = false,
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Alias to decimal
        /// </remarks>
        public static DataType Numeric
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameNumeric,
                    Type = typeof(Decimal),
                    SqlDbType = SqlDbType.Decimal,
                    Scale = 38,
                    Precision = 38,
                    Size = 17,
                    HasSize = false,
                    MaxSize = 17,
                    VarSize = false,
                };
            }
        }

        public static DataType Real
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameReal,
                    Type = typeof(Single),
                    SqlDbType = SqlDbType.Real,
                    Scale = 0,
                    Precision = 24,
                    Size = 4,
                    HasSize = false,
                    MaxSize = 4,
                    VarSize = false,
                };
            }
        }

        public static DataType Float
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameFloat,
                    Type = typeof(Double),
                    SqlDbType = SqlDbType.Float,
                    Scale = 0,
                    Precision = 53,
                    Size = 8,
                    HasSize = false,
                    MaxSize = 8,
                    VarSize = false,
                };
            }
        }

        public static DataType Date
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameDate,
                    Type = typeof(DateTime),
                    SqlDbType = SqlDbType.Date,
                    Scale = 0,
                    Precision = 10,
                    Size = 3,
                    HasSize = false,
                    MaxSize = 3,
                    VarSize = false,
                };
            }
        }

        public static DataType Time
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameTime,
                    Type = typeof(DateTime),
                    SqlDbType = SqlDbType.Time,
                    Scale = 7,
                    Precision = 16,
                    Size = 5,
                    HasSize = false,
                    MaxSize = 5,
                    VarSize = false,
                };
            }
        }

        public static DataType SmallDateTime
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameSmallDateTime,
                    Type = typeof(DateTime),
                    SqlDbType = SqlDbType.SmallDateTime,
                    Scale = 0,
                    Precision = 16,
                    Size = 4,
                    HasSize = false,
                    MaxSize = 4,
                    VarSize = false,
                };
            }
        }

        public static DataType DateTime
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameDateTime,
                    Type = typeof(DateTime),
                    SqlDbType = SqlDbType.DateTime,
                    Scale = 3,
                    Precision = 23,
                    Size = 8,
                    HasSize = false,
                    MaxSize = 8,
                    VarSize = false,
                };
            }
        }

        public static DataType DateTime2
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameDateTime2,
                    Type = typeof(DateTime),
                    SqlDbType = SqlDbType.DateTime2,
                    Scale = 7,
                    Precision = 27,
                    Size = 8,
                    HasSize = false,
                    MaxSize = 8,
                    VarSize = false,
                };
            }
        }

        public static DataType DateTimeOffset
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameDateTimeOffset,
                    Type = typeof(TimeSpan),
                    SqlDbType = SqlDbType.DateTimeOffset,
                    Scale = 7,
                    Precision = 34,
                    Size = 10,
                    HasSize = false,
                    MaxSize = 10,
                    VarSize = false,
                };
            }
        }

        public static DataType Char
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameChar,
                    Type = typeof(String),
                    SqlDbType = SqlDbType.Char,
                    Scale = 0,
                    Precision = 0,
                    Size = 1,
                    HasSize = true,
                    MaxSize = 8000,
                    VarSize = true,
                };
            }
        }

        public static DataType VarChar
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameVarChar,
                    Type = typeof(String),
                    SqlDbType = SqlDbType.VarChar,
                    Scale = 0,
                    Precision = 0,
                    Size = 1,
                    HasSize = true,
                    MaxSize = 8000,
                    VarSize = true,
                };
            }
        }

        public static DataType Text
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameText,
                    Type = typeof(String),
                    SqlDbType = SqlDbType.Text,
                    Scale = 0,
                    Precision = 0,
                    Size = 1,
                    HasSize = true,
                    MaxSize = 16,
                    VarSize = true,
                };
            }
        }

        public static DataType NChar
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameNChar,
                    Type = typeof(String),
                    SqlDbType = SqlDbType.NChar,
                    Scale = 0,
                    Precision = 0,
                    Size = 1,
                    HasSize = true,
                    MaxSize = 4000,
                    VarSize = true,
                };
            }
        }

        public static DataType NVarChar
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameNVarChar,
                    Type = typeof(String),
                    SqlDbType = SqlDbType.NVarChar,
                    Scale = 0,
                    Precision = 0,
                    Size = 1,
                    HasSize = true,
                    MaxSize = 4000,
                    VarSize = true,
                };
            }
        }

        public static DataType NText
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameNText,
                    Type = typeof(String),
                    SqlDbType = SqlDbType.NText,
                    Scale = 0,
                    Precision = 0,
                    Size = 1,
                    HasSize = true,
                    MaxSize = 16,
                    VarSize = true,
                };
            }
        }

        public static DataType Xml
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameXml,
                    Type = typeof(String),
                    SqlDbType = SqlDbType.Xml,
                    Scale = 0,
                    Precision = 0,
                    Size = 1,
                    HasSize = false,
                    MaxSize = -1,
                    VarSize = true,
                };
            }
        }

        public static DataType Binary
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameBinary,
                    Type = typeof(Byte[]),
                    SqlDbType = SqlDbType.Binary,
                    Scale = 0,
                    Precision = 0,
                    Size = 1,
                    HasSize = true,
                    MaxSize = 8000,
                    VarSize = true,
                };
            }
        }

        public static DataType VarBinary
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameVarBinary,
                    Type = typeof(Byte[]),
                    SqlDbType = SqlDbType.VarBinary,
                    Scale = 0,
                    Precision = 0,
                    Size = 1,
                    HasSize = true,
                    MaxSize = 8000,
                    VarSize = true,
                };
            }
        }

        public static DataType Image
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameImage,
                    Type = typeof(Byte[]),
                    SqlDbType = SqlDbType.Image,
                    Scale = 0,
                    Precision = 0,
                    Size = 1,
                    HasSize = false,
                    MaxSize = 16,
                    VarSize = true,
                };
            }
        }

        public static DataType SqlVariant
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameSqlVariant,
                    Type = typeof(object),
                    SqlDbType = SqlDbType.Variant,
                    Scale = 0,
                    Precision = 0,
                    Size = 1,
                    HasSize = false,
                    MaxSize = 8016,
                    VarSize = true,
                };
            }
        }

        public static DataType Timestamp
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameTimestamp,
                    Type = typeof(Int64),
                    SqlDbType = SqlDbType.Timestamp,
                    Scale = 0,
                    Precision = 0,
                    Size = 8,
                    HasSize = false,
                    MaxSize = 8,
                    VarSize = false,
                };
            }
        }

        public static DataType UniqueIdentifier
        {
            get
            {
                return new DataType()
                {
                    Name = Constants.TypeNameUniqueIdentifier,
                    Type = typeof(Guid),
                    SqlDbType = SqlDbType.UniqueIdentifier,
                    Scale = 0,
                    Precision = 0,
                    Size = 16,
                    HasSize = false,
                    MaxSize = 16,
                    VarSize = false,
                };
            }
        }

        // TODO: missing: cursor, hierarchyid, table

        public static DataType Create(Type type, int size)
        {
            DataType res;

            if (type == typeof(sbyte))
            {
                res = DataType.TinyInt;
            }
            else if (type == typeof(Int16))
            {
                res = DataType.SmallInt;
            }
            else if (type == typeof(Int32))
            {
                res = DataType.Int;
            }
            else if (type == typeof(Int64))
            {
                res = DataType.BigInt;
            }
            else if (type == typeof(byte))
            {
                // SQL Server returns byte for tinyint for some reason
                var dt = DataType.TinyInt;
                dt.type = typeof(byte);
                res = dt;
            }
            else if (type == typeof(UInt16))
            {
                res = DataType.SmallInt;
            }
            else if (type == typeof(UInt32))
            {
                res = DataType.Int;
            }
            else if (type == typeof(UInt64))
            {
                res = DataType.BigInt;
            }
            else if (type == typeof(bool))
            {
                res = DataType.Bit;
            }
            else if (type == typeof(float))
            {
                res = DataType.Real;
            }
            else if (type == typeof(double))
            {
                res = DataType.Float;
            }
            else if (type == typeof(decimal))
            {
                res = DataType.Money;
            }
            else if (type == typeof(char))
            {
                var dt = DataType.Char;
                dt.Size = 1;
                res = dt;
            }
            else if (type == typeof(string))
            {
                res = DataType.NVarChar;
            }
            else if (type == typeof(DateTime))
            {
                res = DataType.DateTime;
            }
            else if (type == typeof(Guid))
            {
                res = DataType.UniqueIdentifier;
            }
            else if (type == typeof(byte[]))
            {
                res = DataType.VarBinary;
            }
            else
            {
                throw new NotImplementedException();
            }

            if (res.hasSize)
            {
                // SQL Server return 2G for max length columns
                if (size > 8000)
                {
                    res.size = -1;
                }
                else
                {
                    res.size = size;
                }
            }

            return res;
        }

        public static DataType Create(DataRow dr)
        {
            var type = Create(
                (Type)dr[SchemaTableColumn.DataType],
                Convert.ToInt32(dr[SchemaTableColumn.ColumnSize]));

            type.Precision = Convert.ToInt16(dr[SchemaTableColumn.NumericPrecision]);
            type.Scale = Convert.ToInt16(dr[SchemaTableColumn.NumericScale]);
            
            // TODO: delete ???
            //type.isMax = (bool)dr[Schema.Constants.SchemaColumnIsLong] || type.size == -1;
            //type.Name = dr[Schema.Constants.SchemaColumnProviderSpecificDataType];
            //this.Name = dr[Schema.Constants.SchemaColumnProviderType];

            /* 
            if (col.Type == typeof(string) && col.Size == 1)
            {
                col.Type = typeof(char);
            }*/

            return type;
        }

        private string name;
        private Type type;
        private SqlDbType sqlDbType;
        private short scale;
        private short precision;
        private int size;
        private bool hasSize;
        private short maxSize;
        private bool varSize;

        public string Name
        {
            get { return name; }
            private set { name = value; }
        }

        public string NameWithSize
        {
            get
            {
                if (!hasSize)
                {
                    return name;
                }
                else if (size == -1)
                {
                    return String.Format("{0} (max)", name);
                }
                else
                {
                    return String.Format("{0} ({1})", name, size);
                }
            }
        }

        public Type Type
        {
            get { return type; }
            set { type = value; }
        }

        public SqlDbType SqlDbType
        {
            get { return sqlDbType; }
            set { sqlDbType = value; }
        }

        public short Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public short Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

        public bool HasSize
        {
            get { return hasSize; }
            protected set { hasSize = value; }
        }

        public bool IsMax
        {
            get { return size == -1; }
        }

        public short MaxSize
        {
            get { return maxSize; }
            set { maxSize = value; }
        }

        public bool VarSize
        {
            get { return varSize; }
            set { varSize = value; }
        }

        public bool IsSigned
        {
            get
            {
                switch (sqlDbType)
                {
                    case System.Data.SqlDbType.BigInt:
                    case System.Data.SqlDbType.Decimal:
                    case System.Data.SqlDbType.Float:
                    case System.Data.SqlDbType.Int:
                    case System.Data.SqlDbType.Money:
                    case System.Data.SqlDbType.Real:
                    case System.Data.SqlDbType.SmallInt:
                    case System.Data.SqlDbType.SmallMoney:
                        return true;
                    case System.Data.SqlDbType.Bit:
                    case System.Data.SqlDbType.Binary:
                    case System.Data.SqlDbType.Char:
                    case System.Data.SqlDbType.Date:
                    case System.Data.SqlDbType.DateTime:
                    case System.Data.SqlDbType.DateTime2:
                    case System.Data.SqlDbType.DateTimeOffset:
                    case System.Data.SqlDbType.Image:
                    case System.Data.SqlDbType.NChar:
                    case System.Data.SqlDbType.NText:
                    case System.Data.SqlDbType.NVarChar:
                    case System.Data.SqlDbType.SmallDateTime:
                    case System.Data.SqlDbType.Structured:
                    case System.Data.SqlDbType.Text:
                    case System.Data.SqlDbType.Time:
                    case System.Data.SqlDbType.Timestamp:
                    case System.Data.SqlDbType.TinyInt:
                    case System.Data.SqlDbType.Udt:
                    case System.Data.SqlDbType.UniqueIdentifier:
                    case System.Data.SqlDbType.VarBinary:
                    case System.Data.SqlDbType.VarChar:
                    case System.Data.SqlDbType.Variant:
                    case System.Data.SqlDbType.Xml:
                        return false;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public bool IsInteger
        {
            get
            {
                switch (sqlDbType)
                {
                    case System.Data.SqlDbType.BigInt:
                    case System.Data.SqlDbType.Int:
                    case System.Data.SqlDbType.SmallInt:
                    case System.Data.SqlDbType.TinyInt:
                        return true;
                    case System.Data.SqlDbType.Decimal:
                    case System.Data.SqlDbType.Float:                    
                    case System.Data.SqlDbType.Money:
                    case System.Data.SqlDbType.Real:
                    case System.Data.SqlDbType.SmallMoney:
                    case System.Data.SqlDbType.Bit:
                    case System.Data.SqlDbType.Binary:
                    case System.Data.SqlDbType.Char:
                    case System.Data.SqlDbType.Date:
                    case System.Data.SqlDbType.DateTime:
                    case System.Data.SqlDbType.DateTime2:
                    case System.Data.SqlDbType.DateTimeOffset:
                    case System.Data.SqlDbType.Image:
                    case System.Data.SqlDbType.NChar:
                    case System.Data.SqlDbType.NText:
                    case System.Data.SqlDbType.NVarChar:
                    case System.Data.SqlDbType.SmallDateTime:
                    case System.Data.SqlDbType.Structured:
                    case System.Data.SqlDbType.Text:
                    case System.Data.SqlDbType.Time:
                    case System.Data.SqlDbType.Timestamp:
                    case System.Data.SqlDbType.Udt:
                    case System.Data.SqlDbType.UniqueIdentifier:
                    case System.Data.SqlDbType.VarBinary:
                    case System.Data.SqlDbType.VarChar:
                    case System.Data.SqlDbType.Variant:
                    case System.Data.SqlDbType.Xml:
                        return false;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        protected DataType()
        {
            InitializeMembers();
        }

        public DataType(DataType old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.type = null;
            this.sqlDbType = System.Data.SqlDbType.Int;
            this.scale = 0;
            this.precision = 0;
            this.size = 0;
            this.maxSize = 0;
            this.varSize = false;
        }

        private void CopyMembers(DataType old)
        {
            this.name = old.name;
            this.type = old.type;
            this.sqlDbType = old.sqlDbType;
            this.scale = old.scale;
            this.precision = old.precision;
            this.size = old.size;
            this.maxSize = old.maxSize;
            this.varSize = old.varSize;
        }

        public object Clone()
        {
            return new DataType(this);
        }

        public void CopyToSchemaTableRow(DataRow dr)
        {
            dr[SchemaTableColumn.ColumnSize] = this.Size;
            dr[SchemaTableColumn.NumericPrecision] = this.Precision;
            dr[SchemaTableColumn.NumericScale] = this.Scale;
            dr[SchemaTableColumn.DataType] = this.Type;
            dr[SchemaTableColumn.ProviderType] = this.Name;
            dr[SchemaTableColumn.IsLong] = this.IsMax;
            dr[SchemaTableOptionalColumn.ProviderSpecificDataType] = this.Name;
        }
    }
}
