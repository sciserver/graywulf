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
    public class NullableColumnEntity : Entity
    {
        [DbColumn(Binding = DbColumnBinding.Key)]
        public int ID { get; set; }

        [DbColumn]
        public string Name { get; set; }

        // Data type tests

        [DbColumn]
        public Int32? Int32 { get; set; }

        [DbColumn]
        public Double? Double { get; set; }

        [DbColumn]
        public byte[] ByteArray { get; set; }

        [DbColumn]
        public DateTime? DateTime { get; set; }

        [DbColumn]
        public Guid? Guid { get; set; }

        public NullableColumnEntity()
        {
        }

        public NullableColumnEntity(Context context)
            : base(context)
        {
        }
    }
}
