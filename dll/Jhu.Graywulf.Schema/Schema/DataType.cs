using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

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
                    MaxSize = 8000,
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
                    MaxSize = 8000,
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

        public static DataType GetType(string name)
        {
            switch (name.ToLowerInvariant().Trim())
            {
                case Constants.TypeNameTinyInt:
                    return TinyInt;
                case Constants.TypeNameSmallInt:
                    return SmallInt;
                case Constants.TypeNameInt:
                    return Int;
                case Constants.TypeNameBigInt:
                    return BigInt;
                case Constants.TypeNameBit:
                    return Bit;
                case Constants.TypeNameDecimal:
                    return Decimal;
                case Constants.TypeNameSmallMoney:
                    return SmallMoney;
                case Constants.TypeNameMoney:
                    return Money;
                case Constants.TypeNameNumeric:
                    return Numeric;
                case Constants.TypeNameReal:
                    return Real;
                case Constants.TypeNameFloat:
                    return Float;
                case Constants.TypeNameDate:
                    return Date;
                case Constants.TypeNameTime:
                    return Time;
                case Constants.TypeNameSmallDateTime:
                    return SmallDateTime;
                case Constants.TypeNameDateTime:
                    return DateTime;
                case Constants.TypeNameDateTime2:
                    return DateTime2;
                case Constants.TypeNameDateTimeOffset:
                    return DateTimeOffset;
                case Constants.TypeNameChar:
                    return Char;
                case Constants.TypeNameVarChar:
                    return VarChar;
                case Constants.TypeNameText:
                    return Text;
                case Constants.TypeNameNChar:
                    return NChar;
                case Constants.TypeNameNVarChar:
                    return NVarChar;
                case Constants.TypeNameNText:
                    return NText;
                case Constants.TypeNameXml:
                    return Xml;
                case Constants.TypeNameBinary:
                    return Binary;
                case Constants.TypeNameVarBinary:
                    return VarBinary;
                case Constants.TypeNameImage:
                    return Image;
                case Constants.TypeNameSqlVariant:
                    return SqlVariant;
                case Constants.TypeNameTimestamp:
                    return Timestamp;
                case Constants.TypeNameUniqueIdentifier:
                    return UniqueIdentifier;
                default:
                    throw new ArgumentOutOfRangeException("name");
            }
        }

        public static DataType GetType(string name, short size)
        {
            var t = GetType(name);
            t.Size = size;

            return t;
        }

        public static DataType GetType(string name, short size, byte scale, byte precision)
        {
            var t = GetType(name);
            t.Size = size;
            t.Scale = scale;
            t.Precision = precision;

            return t;
        }

        public static DataType GetType(Type type)
        {
            if (type == typeof(sbyte))
            {
                return DataType.TinyInt;
            }
            else if (type == typeof(Int16))
            {
                return DataType.SmallInt;
            }
            else if (type == typeof(Int32))
            {
                return DataType.Int;
            }
            else if (type == typeof(Int64))
            {
                return DataType.BigInt;
            }
            else if (type == typeof(byte))
            {
                // SQL Server returns byte for tinyint for some reason
                var dt = DataType.TinyInt;
                dt.type = typeof(byte);
                return dt;
            }
            else if (type == typeof(UInt16))
            {
                return DataType.SmallInt;
            }
            else if (type == typeof(UInt32))
            {
                return DataType.Int;
            }
            else if (type == typeof(UInt64))
            {
                return DataType.BigInt;
            }
            else if (type == typeof(bool))
            {
                return DataType.Bit;
            }
            else if (type == typeof(float))
            {
                return DataType.Real;
            }
            else if (type == typeof(double))
            {
                return DataType.Float;
            }
            else if (type == typeof(decimal))
            {
                return DataType.Money;
            }
            else if (type == typeof(char))
            {
                var dt = DataType.Char;
                dt.Size = 1;
                return dt;
            }
            else if (type == typeof(string))
            {
                return DataType.NVarChar;
            }
            else if (type == typeof(DateTime))
            {
                return DataType.DateTime;
            }
            else if (type == typeof(Guid))
            {
                return DataType.UniqueIdentifier;
            }
            else if (type == typeof(byte[]))
            {
                return DataType.VarBinary;
            }            
            else
            {
                throw new NotImplementedException();
            }
        }

        public static DataType GetType(DataRow dr)
        {
            var type = GetType((Type)dr[Schema.Constants.SchemaColumnDataType]);

            type.Size = (short)(int)dr[Schema.Constants.SchemaColumnColumnSize];
            type.Precision = (short)dr[Schema.Constants.SchemaColumnNumericPrecision];
            type.Scale = (short)dr[Schema.Constants.SchemaColumnNumericScale];
            
            //type.IsMax = (bool)dr[Schema.Constants.SchemaColumnIsLong];
            //type.Name = dr[Schema.Constants.SchemaColumnProviderSpecificDataType];
            //this.Name = dr[Schema.Constants.SchemaColumnProviderType];

            /* TODO:
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
        private short size;
        private bool hasSize;
        private short maxSize;
        private bool varSize;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string NameWithSize
        {
            get
            {
                if (!hasSize)
                {
                    return name;
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

        public short Size
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
            /* TODO: delete
            set
            {
                if (value && maxSize != -1)
                {
                    throw new InvalidOperationException();
                }
                else if (value)
                {
                    size = -1;
                }
            }*/
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
            dr[Schema.Constants.SchemaColumnColumnSize] = this.Size;
            dr[Schema.Constants.SchemaColumnNumericPrecision] = this.Precision;
            dr[Schema.Constants.SchemaColumnNumericScale] = this.Scale;
            dr[Schema.Constants.SchemaColumnDataType] = this.Type;
            dr[Schema.Constants.SchemaColumnProviderType] = this.Name;
            dr[Schema.Constants.SchemaColumnIsLong] = this.IsMax;
            dr[Schema.Constants.SchemaColumnProviderSpecificDataType] = this.Name;
        }
    }
}
