using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using Jhu.Graywulf.Entities.AccessControl;

namespace Jhu.Graywulf.Entities.Sql
{
    [SqlUserDefinedType(Format.UserDefined, IsByteOrdered = false, MaxByteSize = -1, IsFixedLength = false, Name = "entities.Identity")]
    public struct SqlIdentity : IBinarySerialize, INullable
    {
        private Identity identity;

        public bool IsNull
        {
            get { return identity == null; }
        }

        public static SqlIdentity Null
        {
            get
            {
                return new SqlIdentity();
            }
        }

        public static SqlIdentity Parse(SqlString xml)
        {
            return new SqlIdentity(xml.Value);
        }

        public override string ToString()
        {
            return identity.ToXml();
        }

        public SqlIdentity(string xml)
        {
            this.identity = Identity.FromXml(xml);
        }

        public void Read(System.IO.BinaryReader r)
        {
            identity = Identity.FromBinary(r);
        }

        public void Write(System.IO.BinaryWriter w)
        {
            identity.ToBinary(w);
        }
    }
}
