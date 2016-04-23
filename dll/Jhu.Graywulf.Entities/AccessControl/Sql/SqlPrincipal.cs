using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.SqlServer.Server;
using Jhu.Graywulf.AccessControl;

namespace Jhu.Graywulf.Entities.Sql
{
    [SqlUserDefinedType(Format.UserDefined, IsByteOrdered = false, MaxByteSize = -1, IsFixedLength = false, Name = "entities.Identity")]
    public struct SqlPrincipal : IBinarySerialize, INullable
    {
        private Principal principal;

        public bool IsNull
        {
            get { return principal == null; }
        }

        public static SqlPrincipal Null
        {
            get
            {
                return new SqlPrincipal();
            }
        }

        public bool IsAuthenticated
        {
            get { return principal.Identity.IsAuthenticated; }
        }

        public static SqlPrincipal FromBinary(SqlBytes bytes)
        {
            return new SqlPrincipal(bytes.Value);
        }

        public static SqlPrincipal Parse(SqlString xml)
        {
            return new SqlPrincipal(xml.Value);
        }

        public override string ToString()
        {
            return principal.ToXml();
        }

        public SqlPrincipal(byte[] bytes)
        {
            this.principal = Principal.FromBinary(bytes);
        }

        public SqlPrincipal(string xml)
        {
            this.principal = Principal.FromXml(xml);
        }

        public void Read(System.IO.BinaryReader r)
        {
            principal = Principal.FromBinary(r);
        }

        public void Write(System.IO.BinaryWriter w)
        {
            principal.ToBinary(w);
        }

        public SqlBinary ToBinary()
        {
            return new SqlBinary(principal.ToBinary());
        }

        public SqlBoolean IsOwner(SqlBytes aclbytes)
        {
            var acl = EntityAcl.FromBinary(new BinaryReader(aclbytes.Stream));
            var access = acl.EvaluateAccess(principal);
            return access.IsOwner;
        }

        public SqlBoolean Can(SqlBytes aclbytes, SqlString access)
        {
            var acl = EntityAcl.FromBinary(new BinaryReader(aclbytes.Stream));
            var a = acl.EvaluateAccess(principal);
            return a.Can(access.Value);
        }

        public SqlBoolean CanCreate(SqlBytes aclbytes)
        {
            var acl = EntityAcl.FromBinary(new BinaryReader(aclbytes.Stream));
            var access = acl.EvaluateAccess(principal);
            return access.CanCreate();
        }

        public SqlBoolean CanRead(SqlBytes aclbytes)
        {
            var acl = EntityAcl.FromBinary(new BinaryReader(aclbytes.Stream));
            var access = acl.EvaluateAccess(principal);
            return access.CanRead();
        }

        public SqlBoolean CanUpdate(SqlBytes aclbytes)
        {
            var acl = EntityAcl.FromBinary(new BinaryReader(aclbytes.Stream));
            var access = acl.EvaluateAccess(principal);
            return access.CanUpdate();
        }

        public SqlBoolean CanDelete(SqlBytes aclbytes)
        {
            var acl = EntityAcl.FromBinary(new BinaryReader(aclbytes.Stream));
            var access = acl.EvaluateAccess(principal);
            return access.CanDelete();
        }
    }
}
