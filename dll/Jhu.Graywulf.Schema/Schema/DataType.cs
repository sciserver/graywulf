using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Schema
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class DataType : ICloneable
    {

        #region Static properties to directly create types

        public static DataType Unknown
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameUnknown
                };
            }
        }

        public static DataType SqlTinyInt
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameTinyInt,
                    Type = typeof(Byte),
                    SqlDbType = System.Data.SqlDbType.TinyInt,
                    ByteSize = sizeof(Byte),
                    Scale = 0,
                    Precision = 3
                };
            }
        }

        public static DataType SqlSmallInt
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameSmallInt,
                    Type = typeof(Int16),
                    SqlDbType = System.Data.SqlDbType.SmallInt,
                    ByteSize = sizeof(Int16),
                    Scale = 0,
                    Precision = 5,
                };
            }
        }

        public static DataType SqlInt
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameInt,
                    Type = typeof(Int32),
                    SqlDbType = System.Data.SqlDbType.Int,
                    ByteSize = sizeof(Int32),
                    Scale = 0,
                    Precision = 10
                };
            }
        }

        public static DataType SqlBigInt
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameBigInt,
                    Type = typeof(Int64),
                    SqlDbType = System.Data.SqlDbType.BigInt,
                    ByteSize = sizeof(Int64),
                    Scale = 0,
                    Precision = 19
                };
            }
        }

        public static DataType SqlBit
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameBit,
                    Type = typeof(Boolean),
                    SqlDbType = System.Data.SqlDbType.Bit,
                    ByteSize = 1,   // well, sort of...
                    Scale = 0,
                    Precision = 1,
                };
            }
        }

        public static DataType SqlDecimal
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameDecimal,
                    Type = typeof(Decimal),
                    SqlDbType = System.Data.SqlDbType.Decimal,
                    ByteSize = sizeof(Decimal),
                    Scale = 0,      // default value
                    Precision = 18, // default value
                };
            }
        }

        public static DataType SqlSmallMoney
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameSmallMoney,
                    Type = typeof(Decimal),
                    SqlDbType = System.Data.SqlDbType.SmallMoney,
                    ByteSize = sizeof(Decimal),
                    Scale = 4,
                    Precision = 10
                };
            }
        }

        public static DataType SqlMoney
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameMoney,
                    Type = typeof(Decimal),
                    SqlDbType = System.Data.SqlDbType.Money,
                    ByteSize = sizeof(Decimal),
                    Scale = 4,
                    Precision = 19
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Alias to decimal
        /// </remarks>
        public static DataType SqlNumeric
        {
            get
            {
                return SqlDecimal;
            }
        }

        public static DataType SqlReal
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameReal,
                    Type = typeof(Single),
                    SqlDbType = System.Data.SqlDbType.Real,
                    ByteSize = sizeof(Single),
                    Scale = 0,
                    Precision = 24
                };
            }
        }

        public static DataType SqlFloat
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameFloat,
                    Type = typeof(Double),
                    SqlDbType = System.Data.SqlDbType.Float,
                    ByteSize = sizeof(Double),
                    Scale = 0,
                    Precision = 53,
                };
            }
        }

        public static DataType SqlDate
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameDate,
                    Type = typeof(DateTime),
                    SqlDbType = System.Data.SqlDbType.Date,
                    ByteSize = 8,
                    Scale = 0,
                    Precision = 10,
                };
            }
        }

        public static DataType SqlTime
        {
            // The equivalent of SqlTime is TimeSpan
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameTime,
                    Type = typeof(TimeSpan),
                    SqlDbType = System.Data.SqlDbType.Time,
                    ByteSize = 8,
                    Scale = 7,
                    Precision = 16
                };
            }
        }

        public static DataType SqlSmallDateTime
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameSmallDateTime,
                    Type = typeof(DateTime),
                    SqlDbType = System.Data.SqlDbType.SmallDateTime,
                    ByteSize = 8
                };
            }
        }

        public static DataType SqlDateTime
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameDateTime,
                    Type = typeof(DateTime),
                    SqlDbType = System.Data.SqlDbType.DateTime,
                    ByteSize = 8,
                    Scale = 3,
                    Precision = 23
                };
            }
        }

        public static DataType SqlDateTime2
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameDateTime2,
                    Type = typeof(DateTime),
                    SqlDbType = System.Data.SqlDbType.DateTime2,
                    ByteSize = 8
                };
            }
        }

        public static DataType SqlDateTimeOffset
        {
            // This is not a TimeSpan but a time with a time zone offset!
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameDateTimeOffset,
                    Type = typeof(DateTimeOffset),
                    SqlDbType = System.Data.SqlDbType.DateTimeOffset,
                    ByteSize = 8,
                    Scale = 7,
                    Precision = 34
                };
            }
        }

        public static DataType SqlChar
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameChar,
                    Type = typeof(String),
                    SqlDbType = System.Data.SqlDbType.Char,
                    ByteSize = 1,
                    Scale = 0,
                    Precision = 0,
                    Length = 1,
                    MaxLength = 8000,
                    IsVarLength = false
                };
            }
        }

        public static DataType SqlVarChar
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameVarChar,
                    Type = typeof(String),
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    ByteSize = 1,
                    Scale = 0,
                    Precision = 0,
                    Length = 1,
                    MaxLength = 8000,
                    IsVarLength = true
                };
            }
        }

        public static DataType SqlVarCharMax
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameVarChar,
                    Type = typeof(String),
                    SqlDbType = System.Data.SqlDbType.VarChar,
                    ByteSize = 1,
                    Scale = 0,
                    Precision = 0,
                    Length = -1,
                    MaxLength = -1,
                    IsVarLength = true
                };
            }
        }

        public static DataType SqlText
        {
            // This differs from varchar(max) in internal representations
            // and must be serialized differently into the native format

            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameText,
                    Type = typeof(String),
                    SqlDbType = System.Data.SqlDbType.Text,
                    ByteSize = 1,
                    Scale = 0,
                    Precision = 0,
                    Length = -1,
                    MaxLength = -1,
                    IsVarLength = true
                };
            }
        }

        public static DataType SqlNChar
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameNChar,
                    Type = typeof(String),
                    SqlDbType = System.Data.SqlDbType.NChar,
                    ByteSize = 2,
                    Scale = 0,
                    Precision = 0,
                    Length = 1,
                    MaxLength = 4000,
                    IsVarLength = false
                };
            }
        }

        public static DataType SqlNVarChar
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameNVarChar,
                    Type = typeof(String),
                    SqlDbType = System.Data.SqlDbType.NVarChar,
                    ByteSize = 2,
                    Scale = 0,
                    Precision = 0,
                    Length = 1,
                    MaxLength = 4000,
                    IsVarLength = true
                };
            }
        }

        public static DataType SqlNVarCharMax
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameNVarChar,
                    Type = typeof(String),
                    SqlDbType = System.Data.SqlDbType.NVarChar,
                    ByteSize = 2,
                    Scale = 0,
                    Precision = 0,
                    Length = -1,
                    MaxLength = -1,
                    IsVarLength = true
                };
            }
        }

        public static DataType SqlNText
        {
            // This is stored differently than nvarchar(max) and prefix is
            // different for native serialization

            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameNText,
                    Type = typeof(String),
                    SqlDbType = System.Data.SqlDbType.NText,
                    ByteSize = 2,
                    Scale = 0,
                    Precision = 0,
                    Length = -1,
                    MaxLength = -1,
                    IsVarLength = true
                };
            }
        }

        public static DataType SqlXml
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static DataType SqlBinary
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameBinary,
                    Type = typeof(Byte[]),
                    SqlDbType = System.Data.SqlDbType.Binary,
                    ByteSize = 1,
                    Scale = 0,
                    Precision = 0,
                    Length = 1,
                    MaxLength = 8000,
                    IsVarLength = false
                };
            }
        }

        public static DataType SqlVarBinary
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameVarBinary,
                    Type = typeof(Byte[]),
                    SqlDbType = System.Data.SqlDbType.VarBinary,
                    ByteSize = 1,
                    Scale = 0,
                    Precision = 0,
                    Length = 1,
                    MaxLength = 8000,
                    IsVarLength = true
                };
            }
        }

        public static DataType SqlVarBinaryMax
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameVarBinary,
                    Type = typeof(Byte[]),
                    SqlDbType = System.Data.SqlDbType.VarBinary,
                    ByteSize = 1,
                    Scale = 0,
                    Precision = 0,
                    Length = -1,
                    MaxLength = -1,
                    IsVarLength = true
                };
            }
        }

        public static DataType SqlImage
        {
            // This is different internally from varbinary(max)
            // It has a shorter prefix in native format.

            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameImage,
                    Type = typeof(Byte[]),
                    SqlDbType = System.Data.SqlDbType.Image,
                    ByteSize = 1,
                    Scale = 0,
                    Precision = 0,
                    Length = -1,
                    MaxLength = -1,
                    IsVarLength = true
                };
            }
        }

        public static DataType SqlVariant
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public static DataType SqlTimestamp
        {
            // SQL timestamp columns is byte[8]
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameTimestamp,
                    Type = typeof(Byte[]),
                    SqlDbType = System.Data.SqlDbType.Timestamp,
                    ByteSize = 8,
                    Scale = 0,
                    Precision = 0
                };
            }
        }

        public static DataType SqlUniqueIdentifier
        {
            get
            {
                return new DataType()
                {
                    Name = SqlServer.Constants.TypeNameUniqueIdentifier,
                    Type = typeof(Guid),
                    SqlDbType = System.Data.SqlDbType.UniqueIdentifier,
                    ByteSize = 16,
                    Scale = 0,
                    Precision = 0
                };
            }
        }

        // TODO: missing: cursor, hierarchyid, table

        // --- .Net types

        public static DataType Boolean
        {
            get
            {
                return SqlBit;
            }
        }

        public static DataType SByte
        {
            get
            {
                return new DataType()
                {
                    Name = null,
                    Type = typeof(SByte),
                    SqlDbType = null,
                    ByteSize = 1,
                    Scale = 0,
                    Precision = 3
                };

            }
        }

        public static DataType Byte
        {
            get
            {
                return SqlTinyInt;
            }
        }

        public static DataType Int16
        {
            get
            {
                return SqlSmallInt;
            }
        }

        public static DataType UInt16
        {
            get
            {
                return new DataType()
                {
                    Name = null,
                    Type = typeof(UInt16),
                    SqlDbType = null,
                    ByteSize = sizeof(UInt16),
                    Scale = 0,
                    Precision = 5,
                };
            }
        }

        public static DataType Int32
        {
            get
            {
                return SqlInt;
            }
        }

        public static DataType UInt32
        {
            get
            {
                return new DataType()
                {
                    Name = null,
                    Type = typeof(UInt32),
                    SqlDbType = null,
                    ByteSize = sizeof(UInt32),
                    Scale = 0,
                    Precision = 10
                };
            }
        }

        public static DataType Int64
        {
            get
            {
                return SqlBigInt;
            }
        }

        public static DataType UInt64
        {
            get
            {
                return new DataType()
                {
                    Name = null,
                    Type = typeof(UInt64),
                    SqlDbType = null,
                    ByteSize = sizeof(UInt64),
                    Scale = 0,
                    Precision = 19
                };
            }
        }

        public static DataType Single
        {
            get { return SqlReal; }
        }

        public static DataType Double
        {
            get { return SqlFloat; }
        }

        public static DataType Decimal
        {
            get { return SqlMoney; }
        }

        public static DataType DateTime
        {
            get { return SqlDateTime2; }
        }

        public static DataType DateTimeOffset
        {
            get { return SqlDateTimeOffset; }
        }

        public static DataType Guid
        {
            get { return SqlUniqueIdentifier; }
        }

        public static DataType SingleComplex
        {
            get
            {
                return new DataType()
                {
                    Name = null,
                    Type = typeof(SingleComplex),
                    SqlDbType = null,
                    ByteSize = 8,
                    Scale = 0,
                    Precision = 24
                };
            }
        }

        public static DataType DoubleComplex
        {
            get
            {
                return new DataType()
                {
                    Name = null,
                    Type = typeof(DoubleComplex),
                    SqlDbType = null,
                    ByteSize = 16,
                    Scale = 0,
                    Precision = 53,
                };
            }
        }

        public static DataType Char
        {
            get { return SqlNChar; }
        }

        public static DataType String
        {
            get { return SqlNVarChar; }
        }

        #endregion
        #region Static functions to create types

        public static DataType Create(SqlDbType type, int length, byte precision, byte scale, bool isNullable)
        {
            DataType dt;

            switch (type)
            {
                case System.Data.SqlDbType.Bit:
                    dt = DataType.SqlBit;
                    break;
                case System.Data.SqlDbType.TinyInt:
                    dt = DataType.SqlTinyInt;
                    break;
                case System.Data.SqlDbType.SmallInt:
                    dt = DataType.SqlSmallInt;
                    break;
                case System.Data.SqlDbType.Int:
                    dt = DataType.SqlInt;
                    break;
                case System.Data.SqlDbType.BigInt:
                    dt = DataType.SqlBigInt;
                    break;
                case System.Data.SqlDbType.Real:
                    dt = DataType.SqlReal;
                    break;
                case System.Data.SqlDbType.Float:
                    dt = DataType.SqlFloat;
                    break;
                case System.Data.SqlDbType.Image:
                    dt = DataType.SqlImage;
                    break;
                case System.Data.SqlDbType.Binary:
                    dt = DataType.SqlBinary;
                    break;
                case System.Data.SqlDbType.VarBinary:
                    dt = DataType.SqlVarBinary;
                    break;
                case System.Data.SqlDbType.Text:
                    dt = DataType.SqlText;
                    break;
                case System.Data.SqlDbType.Char:
                    dt = DataType.SqlChar;
                    break;
                case System.Data.SqlDbType.VarChar:
                    dt = DataType.SqlVarChar;
                    break;
                case System.Data.SqlDbType.NText:
                    dt = DataType.SqlNText;
                    break;
                case System.Data.SqlDbType.NChar:
                    dt = DataType.SqlNChar;
                    break;
                case System.Data.SqlDbType.NVarChar:
                    dt = DataType.SqlNVarChar;
                    break;
                case System.Data.SqlDbType.Date:
                    dt = DataType.SqlDate;
                    break;
                case System.Data.SqlDbType.DateTime:
                    dt = DataType.SqlDateTime;
                    break;
                case System.Data.SqlDbType.DateTime2:
                    dt = DataType.SqlDateTime2;
                    break;
                case System.Data.SqlDbType.DateTimeOffset:
                    dt = DataType.SqlDateTimeOffset;
                    break;
                case System.Data.SqlDbType.SmallDateTime:
                    dt = DataType.SqlSmallDateTime;
                    break;
                case System.Data.SqlDbType.Time:
                    dt = DataType.SqlTime;
                    break;
                case System.Data.SqlDbType.Timestamp:
                    dt = DataType.SqlTimestamp;
                    break;
                case System.Data.SqlDbType.Decimal:
                    dt = DataType.SqlDecimal;
                    dt.Precision = precision;
                    dt.Scale = scale;
                    break;
                case System.Data.SqlDbType.SmallMoney:
                    dt = DataType.SqlSmallMoney;
                    break;
                case System.Data.SqlDbType.Money:
                    dt = DataType.SqlMoney;
                    break;
                case System.Data.SqlDbType.UniqueIdentifier:
                    dt = DataType.SqlUniqueIdentifier;
                    break;
                case System.Data.SqlDbType.Variant:
                    dt = DataType.SqlVariant;
                    break;
                case System.Data.SqlDbType.Xml:
                    dt = DataType.SqlXml;
                    break;
                case System.Data.SqlDbType.Structured:
                    throw new NotImplementedException();
                case System.Data.SqlDbType.Udt:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }

            if (dt.HasLength)
            {
                dt.Length = length;
            }

            dt.IsNullable = isNullable;

            return dt;
        }

        /// <summary>
        /// Creates data type descriptor from a .Net type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static DataType Create(Type type, int length)
        {
            if (type == typeof(Boolean))
            {
                return DataType.Boolean;
            }
            else if (type == typeof(SByte))
            {
                return DataType.SByte;
            }
            else if (type == typeof(Byte))
            {
                return DataType.Byte;
            }
            else if (type == typeof(Int16))
            {
                return DataType.Int16;
            }
            else if (type == typeof(UInt16))
            {
                return DataType.UInt16;
            }
            else if (type == typeof(Int32))
            {
                return DataType.Int32;
            }
            else if (type == typeof(UInt32))
            {
                return DataType.UInt32;
            }
            else if (type == typeof(Int64))
            {
                return DataType.Int64;
            }
            else if (type == typeof(UInt64))
            {
                return DataType.UInt64;
            }
            else if (type == typeof(Single))
            {
                return DataType.Single;
            }
            else if (type == typeof(Double))
            {
                return DataType.Double;
            }
            else if (type == typeof(Decimal))
            {
                return DataType.Decimal;
            }
            else if (type == typeof(DateTime))
            {
                return DataType.DateTime;
            }
            else if (type == typeof(Guid))
            {
                return DataType.Guid;
            }
            else if (type == typeof(char))
            {
                return DataType.Char;
            }
            else if (type == typeof(char[]))
            {
                var dt = DataType.String;
                dt.Length = length;
                return dt;
            }
            else if (type == typeof(string))
            {
                var dt = DataType.SqlNVarChar;
                dt.Length = length;
                return dt;
            }
            else if (type == typeof(byte[]))
            {
                var dt = DataType.SqlVarBinary;
                dt.Length = length;
                return dt;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static DataType Create(DataRow dr)
        {
            // Get .Net type and other parameters
            var type = (Type)dr[SchemaTableColumn.DataType];
            var length = Convert.ToInt32(dr[SchemaTableColumn.ColumnSize]);
            var precision = Convert.ToByte(dr[SchemaTableColumn.NumericPrecision]);
            var scale = Convert.ToByte(dr[SchemaTableColumn.NumericScale]);
            var isnullable = Convert.ToBoolean(dr[SchemaTableColumn.AllowDBNull]);

            DataType dt;

            // Try to interpret provider type as sql server type
            SqlDbType sqltype;
            if (Enum.TryParse<SqlDbType>((string)dr["DataTypeName"], true, out sqltype))
            {
                // This can be interpreted as a SQL Server type
                dt = Create(sqltype, length, precision, scale, isnullable);
            }
            else
            {
                // This is a .Net type, might not be supported by SqlServer
                dt = Create(type, length);
            }

            return dt;
        }

        #endregion
        #region Private variables for property storage

        /// <summary>
        /// Type name
        /// </summary>
        [NonSerialized]
        private string name;

        /// <summary>
        /// Corresponding .Net type
        /// </summary>
        [NonSerialized]
        private Type type;

        /// <summary>
        /// Corresponding SqlServer type
        /// </summary>
        [NonSerialized]
        private SqlDbType? sqlDbType;

        /// <summary>
        /// Size of the primitive type in bytes
        /// </summary>
        [NonSerialized]
        private int byteSize;

        /// <summary>
        /// Scale (for decimal)
        /// </summary>
        [NonSerialized]
        private byte scale;

        /// <summary>
        /// Precision (for decimal)
        /// </summary>
        [NonSerialized]
        private byte precision;

        /// <summary>
        /// Size in bytes (for char and binary)
        /// </summary>
        [NonSerialized]
        private int length;

        /// <summary>
        /// Maximum length in SQL Server, -1 means max
        /// </summary>
        [NonSerialized]
        private int maxLength;

        /// <summary>
        /// Is length variable (char, binary vs varchar, varbinary)
        /// </summary>
        [NonSerialized]
        private bool isVarLength;

        /// <summary>
        /// Is an array, currently no SQL Server support
        /// </summary>
        [NonSerialized]
        private bool isSqlArray;

        /// <summary>
        /// Length of array, (not supported by SQL Server)
        /// </summary>
        [NonSerialized]
        private int arrayLength;

        /// <summary>
        /// Is variable length array
        /// </summary>
        [NonSerialized]
        private bool isVarArrayLength;

        /// <summary>
        /// Is type nullable
        /// </summary>
        [NonSerialized]
        private bool isNullable;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the name of the data type.
        /// </summary>
        [DataMember]
        public string Name
        {
            get { return name; }
            private set { name = value; }
        }

        /// <summary>
        /// Gets the SQL name of the type with the length appended.
        /// </summary>
        /// <remarks>
        /// Type name is returned in SQL Server format, e.g. nvarchar(50)
        /// </remarks>
        [IgnoreDataMember]
        public string NameWithLength
        {
            get
            {
                if (!HasLength)
                {
                    return name;
                }
                else if (IsMaxLength)
                {
                    return System.String.Format("{0}(max)", name);
                }
                else
                {
                    return System.String.Format("{0}({1})", name, length);
                }
            }
        }

        /// <summary>
        /// Gets the corresponding .Net type
        /// </summary>
        [IgnoreDataMember]
        public Type Type
        {
            get { return type; }
            private set { type = value; }
        }

        [DataMember]
        private string Type_ForXml
        {
            get { return type != null ? type.FullName : null; }
            set { type = value != null ? Type.GetType(value) : null; }
        }

        /// <summary>
        /// Gets the corresponding SQL Server type
        /// </summary>
        [DataMember]
        public SqlDbType? SqlDbType
        {
            get { return sqlDbType; }
            private set { sqlDbType = value; }
        }

        /// <summary>
        /// Gets the size of the primitive type in bytes
        /// </summary>
        [DataMember]
        public int ByteSize
        {
            get { return byteSize; }
            private set { byteSize = value; }
        }

        /// <summary>
        /// Gets or sets the scale (for decimal values)
        /// </summary>
        [DataMember]
        public byte Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Gets or sets the precision (for decimal values)
        /// </summary>
        [DataMember]
        public byte Precision
        {
            get { return precision; }
            set { precision = value; }
        }

        /// <summary>
        /// Gets or sets the length of the type (for char, etc.)
        /// </summary>
        [DataMember]
        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        /// <summary>
        /// Gets if the length parameter is set to max (varchar(max), etc.)
        /// </summary>
        [IgnoreDataMember]
        public bool IsMaxLength
        {
            get { return length == -1 || (long)length * byteSize > 8000; }
        }

        /// <summary>
        /// Gets the maximum length of the type, in SQL Server
        /// </summary>
        [DataMember]
        public int MaxLength
        {
            get { return maxLength; }
            private set { maxLength = value; }
        }

        /// <summary>
        /// Gets if the length is variable (varchar, varbinary, etc.)
        /// </summary>
        [DataMember]
        public bool IsVarLength
        {
            get { return isVarLength; }
            private set { isVarLength = value; }
        }

        /// <summary>
        /// Gets if the type is an array.
        /// </summary>
        [DataMember]
        public bool IsSqlArray
        {
            get { return isSqlArray; }
            private set { isSqlArray = value; }
        }

        /// <summary>
        /// Gets or sets the maximum array size.
        /// </summary>
        [DataMember]
        public int ArrayLength
        {
            get { return arrayLength; }
            set { arrayLength = value; }
        }

        /// <summary>
        /// Gets or sets if the array is bounded
        /// </summary>
        [DataMember]
        private bool IsVarArrayLength
        {
            get { return isVarArrayLength; }
            set { isVarArrayLength = value; }
        }

        /// <summary>
        /// Gets or sets whether the data type is nullable
        /// </summary>
        [DataMember]
        public bool IsNullable
        {
            get { return isNullable; }
            set { isNullable = value; }
        }

        /// <summary>
        /// Gets if type is compatible with SQL Server
        /// </summary>
        [IgnoreDataMember]
        public bool IsSqlServerCompatible
        {
            get { return sqlDbType.HasValue; }
        }

        /// <summary>
        /// Gets if the type has a length parameter (char, binary, etc.)
        /// </summary>
        [IgnoreDataMember]
        public bool HasLength
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
                    case System.Data.SqlDbType.Bit:
                    case System.Data.SqlDbType.Date:
                    case System.Data.SqlDbType.DateTime:
                    case System.Data.SqlDbType.DateTime2:
                    case System.Data.SqlDbType.DateTimeOffset:
                    case System.Data.SqlDbType.Image:
                    case System.Data.SqlDbType.NText:
                    case System.Data.SqlDbType.SmallDateTime:
                    case System.Data.SqlDbType.Structured:
                    case System.Data.SqlDbType.Text:
                    case System.Data.SqlDbType.Time:
                    case System.Data.SqlDbType.Timestamp:
                    case System.Data.SqlDbType.TinyInt:
                    case System.Data.SqlDbType.Udt:
                    case System.Data.SqlDbType.UniqueIdentifier:
                    case System.Data.SqlDbType.Variant:
                    case System.Data.SqlDbType.Xml:
                        return false;
                    case System.Data.SqlDbType.Char:
                    case System.Data.SqlDbType.VarChar:
                    case System.Data.SqlDbType.NChar:
                    case System.Data.SqlDbType.NVarChar:
                    case System.Data.SqlDbType.Binary:
                    case System.Data.SqlDbType.VarBinary:
                        return true;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Gets if the corresponding SQL Server type is signed
        /// </summary>
        [IgnoreDataMember]
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

        /// <summary>
        /// Gets if the corresponding SQL Server type is integer
        /// </summary>
        [IgnoreDataMember]
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

        // TODO: add properties for HasPrecision and HasScale?

        #endregion
        #region Constructors and initializers

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
            this.byteSize = 0;
            this.scale = 0;
            this.precision = 0;
            this.length = 0;
            this.maxLength = 0;
            this.isVarLength = false;
            this.isSqlArray = false;
            this.arrayLength = 0;
            this.isVarArrayLength = false;
            this.isNullable = false;
        }

        private void CopyMembers(DataType old)
        {
            this.name = old.name;
            this.type = old.type;
            this.sqlDbType = old.sqlDbType;
            this.byteSize = old.byteSize;
            this.scale = old.scale;
            this.precision = old.precision;
            this.length = old.length;
            this.maxLength = old.maxLength;
            this.isVarLength = old.isVarLength;
            this.isSqlArray = old.isSqlArray;
            this.arrayLength = old.arrayLength;
            this.isVarArrayLength = old.isVarArrayLength;
            this.isNullable = old.isNullable;
        }

        public object Clone()
        {
            return new DataType(this);
        }

        #endregion

        public bool Compare(DataType other)
        {
            var res = true;

            res &= SchemaManager.Comparer.Compare(this.Name, other.name) == 0;
            res &= this.type == other.type;
            res &= this.scale == other.scale;
            res &= this.precision == other.precision;
            res &= !this.HasLength || (this.length == other.length);
            res &= this.isNullable == other.isNullable;

            return res;
        }

        public void CopyToSchemaTableRow(DataRow dr)
        {
            if (HasLength)
            {
                dr[SchemaTableColumn.ColumnSize] = this.length;
            }
            else
            {
                dr[SchemaTableColumn.ColumnSize] = this.byteSize;
            }
            dr[SchemaTableColumn.NumericPrecision] = this.precision;
            dr[SchemaTableColumn.NumericScale] = this.scale;
            dr[SchemaTableColumn.DataType] = this.type;
            dr[SchemaTableColumn.ProviderType] = this.name;
            dr[SchemaTableColumn.IsLong] = this.IsMaxLength;
            dr[SchemaTableOptionalColumn.ProviderSpecificDataType] = this.name;
            dr[SchemaTableColumn.AllowDBNull] = this.isNullable;
        }
    }
}
