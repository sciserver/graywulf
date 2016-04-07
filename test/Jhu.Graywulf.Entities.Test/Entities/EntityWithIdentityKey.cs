using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Entities.Mapping;

namespace Jhu.Graywulf.Entities
{
    [DbTable]
    class EntityWithIdentityKey : Entity
    {
        [DbColumn(Binding = DbColumnBinding.Key | DbColumnBinding.Identity, DefaultValue = -1)]
        public int ID { get; set; }

        [DbColumn]
        public string Name { get; set; }

        [DbColumn(Name = "SomethingElse")]
        public string Rename { get; set; }

        [DbColumn(Order = 4)]
        public string Four { get; set; }

        [DbColumn(Type = SqlDbType.VarChar)]
        public string AnsiText { get; set; }

        [DbColumn(Size = 10)]
        public string VarCharText { get; set; }

        [DbColumn(Binding = DbColumnBinding.Auxiliary)]
        public string ReadOnly { get; set; }

        // Data type tests

        [DbColumn]
        public SByte SByte { get; set; }

        [DbColumn]
        public Int16 Int16 { get; set; }

        [DbColumn]
        public Int32 Int32 { get; set; }

        [DbColumn]
        public Int64 Int64 { get; set; }

        [DbColumn]
        public Byte Byte { get; set; }

        [DbColumn]
        public UInt16 UInt16 { get; set; }

        [DbColumn]
        public UInt32 UInt32 { get; set; }

        [DbColumn]
        public UInt64 UInt64 { get; set; }

        [DbColumn]
        public Single Single { get; set; }

        [DbColumn]
        public Double Double { get; set; }

        [DbColumn]
        public Decimal Decimal { get; set; }

        [DbColumn]
        public String String { get; set; }

        [DbColumn]
        public byte[] ByteArray { get; set; }

        [DbColumn]
        public DateTime DateTime { get; set; }

        [DbColumn]
        public Guid Guid { get; set; }

        [DbColumn]
        public XmlElement XmlElement { get; set; }

        public EntityWithIdentityKey()
        {
        }

        public EntityWithIdentityKey(Context context)
            : base(context)
        {
        }
    }
}
