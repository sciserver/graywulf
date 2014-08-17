using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Schema
{
    public static class DataTypes
    {
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
    }
}
