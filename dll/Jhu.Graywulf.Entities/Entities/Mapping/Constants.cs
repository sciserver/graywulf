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
        public static readonly Dictionary<Type, SqlDbType> DbTypeMappings = new Dictionary<Type, SqlDbType>()
        {
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
    }
}
