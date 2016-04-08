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
    [SqlUserDefinedType(Format.UserDefined, IsByteOrdered = false, MaxByteSize = -1, IsFixedLength = false, Name="entities.EntityAcl")]
    public struct SqlEntityAcl : IBinarySerialize
    {
        private EntityAcl acl;

        public static SqlEntityAcl FromXml(SqlChars xml)
        {
            return new SqlEntityAcl(xml);
        }
        
        public SqlEntityAcl(SqlChars xml)
        {
            this.acl = EntityAcl.FromXml(xml.ToString());
        }

        public SqlChars ToXml()
        {
            return new SqlChars(acl.ToXml());
        }

        public void Read(System.IO.BinaryReader r)
        {
            acl = EntityAcl.FromXml(r.BaseStream);
        }

        public void Write(System.IO.BinaryWriter w)
        {
            acl.ToXml(w.BaseStream);
        }
    }
}
