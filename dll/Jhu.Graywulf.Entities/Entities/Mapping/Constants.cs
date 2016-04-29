using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Entities.Mapping
{
    internal class Constants
    {
        public const string IdentityParameterName = "@__identity";
        public const string IdentityVariableName = "@__id";
        public const string FromParameterName = "@__from";
        public const string ToParameterName = "@__to";
        public const string AclColumnName = "__acl";

        public static readonly Dictionary<Type, SqlDbType> TypeToSqlDbType = new Dictionary<Type, SqlDbType>()
        {
            { typeof(Boolean), SqlDbType.Bit },
            { typeof(SByte), SqlDbType.TinyInt },
            { typeof(Int16), SqlDbType.SmallInt },
            { typeof(Int32), SqlDbType.Int },
            { typeof(Int64), SqlDbType.BigInt },
            { typeof(Byte), SqlDbType.TinyInt },
            { typeof(UInt16), SqlDbType.SmallInt },
            { typeof(UInt32), SqlDbType.Int },
            { typeof(UInt64), SqlDbType.BigInt },
            { typeof(Single), SqlDbType.Real },
            { typeof(Double), SqlDbType.Float },
            { typeof(Decimal), SqlDbType.Money },
            { typeof(String), SqlDbType.NVarChar },
            { typeof(Byte[]), SqlDbType.VarBinary },
            { typeof(DateTime), SqlDbType.DateTime },
            { typeof(Guid), SqlDbType.UniqueIdentifier },
            { typeof(XmlElement), SqlDbType.Xml },
        };

        public static readonly Dictionary<SqlDbType, Type> SqlDbTypeToType = new Dictionary<SqlDbType, Type>()
        {
            { SqlDbType.Bit, typeof(Boolean) },
            { SqlDbType.TinyInt, typeof(Byte) },
            { SqlDbType.SmallInt, typeof(Int16) },
            { SqlDbType.Int, typeof(Int32) },
            { SqlDbType.BigInt, typeof(Int64) },
            { SqlDbType.Real, typeof(Single) },
            { SqlDbType.Float, typeof(Double) },
            { SqlDbType.SmallMoney, typeof(Decimal) },
            { SqlDbType.Money, typeof(Decimal) },
            { SqlDbType.Char, typeof(String) },
            { SqlDbType.NChar, typeof(String) },
            { SqlDbType.Text, typeof(String) },
            { SqlDbType.NText, typeof(String) },
            { SqlDbType.VarChar, typeof(String) },
            { SqlDbType.NVarChar, typeof(String) },
            { SqlDbType.Image, typeof(Byte[]) },
            { SqlDbType.Binary, typeof(Byte[]) },
            { SqlDbType.VarBinary, typeof(Byte[]) },
            { SqlDbType.SmallDateTime, typeof(DateTime) },
            { SqlDbType.DateTime, typeof(DateTime) },
            { SqlDbType.DateTime2, typeof(DateTime) },
            { SqlDbType.DateTimeOffset, typeof(TimeSpan) },
            { SqlDbType.UniqueIdentifier, typeof(Guid) },
            { SqlDbType.Xml, typeof(XmlElement) },
        };
    }
}
