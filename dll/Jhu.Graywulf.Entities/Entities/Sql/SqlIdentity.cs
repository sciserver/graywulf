using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
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

        public bool IsAuthenticated
        {
            get { return identity.IsAuthenticated; }
        }

        public static SqlIdentity FromBinary(SqlBytes bytes)
        {
            return new SqlIdentity(bytes.Value);
        }

        public static SqlIdentity Parse(SqlString xml)
        {
            return new SqlIdentity(xml.Value);
        }

        public override string ToString()
        {
            return identity.ToXml();
        }

        public SqlIdentity(byte[] bytes)
        {
            this.identity = Identity.FromBinary(bytes);
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

        public SqlBinary ToBinary()
        {
            return new SqlBinary(identity.ToBinary());
        }

        public SqlBoolean IsOwner(SqlBytes aclbytes)
        {
            var acl = EntityAcl.FromBinary(new BinaryReader(aclbytes.Stream));
            var access = acl.EvaluateAccess(identity);
            return access.IsOwner;
        }

        public SqlBoolean Can(SqlBytes aclbytes, SqlString access)
        {
            var acl = EntityAcl.FromBinary(new BinaryReader(aclbytes.Stream));
            var a = acl.EvaluateAccess(identity);
            return a.Can(access.Value);
        }

        public SqlBoolean CanCreate(SqlBytes aclbytes)
        {
            var acl = EntityAcl.FromBinary(new BinaryReader(aclbytes.Stream));
            var access = acl.EvaluateAccess(identity);
            return access.CanCreate();
        }

        public SqlBoolean CanRead(SqlBytes aclbytes)
        {
            var acl = EntityAcl.FromBinary(new BinaryReader(aclbytes.Stream));
            var access = acl.EvaluateAccess(identity);
            return access.CanRead();
        }

        public SqlBoolean CanUpdate(SqlBytes aclbytes)
        {
            var acl = EntityAcl.FromBinary(new BinaryReader(aclbytes.Stream));
            var access = acl.EvaluateAccess(identity);
            return access.CanUpdate();
        }

        public SqlBoolean CanDelete(SqlBytes aclbytes)
        {
            var acl = EntityAcl.FromBinary(new BinaryReader(aclbytes.Stream));
            var access = acl.EvaluateAccess(identity);
            return access.CanDelete();
        }
    }
}
