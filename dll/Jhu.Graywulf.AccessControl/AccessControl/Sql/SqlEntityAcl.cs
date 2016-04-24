using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.SqlServer.Server;
using Jhu.Graywulf.AccessControl;

namespace Jhu.Graywulf.AccessControl.Sql
{
    [SqlUserDefinedType(Format.UserDefined, IsByteOrdered = false, MaxByteSize = -1, IsFixedLength = false, Name="entities.EntityAcl")]
    public struct SqlEntityAcl : IBinarySerialize, INullable
    {
        private EntityAcl acl;

        public bool IsNull
        {
            get { return acl == null; }
        }

        public static SqlEntityAcl Null
        {
            get
            {
                return new SqlEntityAcl();
            }
        }

        public static SqlEntityAcl FromBinary(SqlBytes bytes)
        {
            return new SqlEntityAcl(bytes.Value);
        }

        public static SqlEntityAcl Parse(SqlString xml)
        {
            return new SqlEntityAcl(xml.Value);
        }

        public override string ToString()
        {
            return acl.ToXml();
        }

        public SqlEntityAcl(byte[] bytes)
        {
            this.acl = EntityAcl.FromBinary(bytes);
        }

        public SqlEntityAcl(string xml)
        {
            this.acl = EntityAcl.FromXml(xml);
        }

        public void Read(System.IO.BinaryReader r)
        {
            acl = EntityAcl.FromBinary(r);
        }

        public void Write(System.IO.BinaryWriter w)
        {
            acl.ToBinary(w);
        }

        public SqlBinary ToBinary()
        {
            return new SqlBinary(acl.ToBinary());
        }
    }
}
