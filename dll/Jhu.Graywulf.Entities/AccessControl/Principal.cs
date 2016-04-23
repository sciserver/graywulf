using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Security.Principal;
using Jhu.Graywulf.Util;

namespace Jhu.Graywulf.AccessControl
{
    public class Principal : IPrincipal, ICloneable
    {
        #region Private member variables

        private Identity identity;
        private PrincipalRoleCollection roles;

        #endregion
        #region Properties

        IIdentity IPrincipal.Identity
        {
            get { return identity; }
        }

        public Identity Identity
        {
            get { return identity; }
            set { identity = value; }
        }

        public PrincipalRoleCollection Roles
        {
            get { return roles; }
        }

        #endregion
        #region Constructors and initializers

        public static Principal Public
        {
            get
            {
                // Authenticated but otherwise unknown
                return new Principal()
                {
                    Identity = new Identity()
                    {
                        IsAuthenticated = true
                    }
                };
            }
        }

        public static Principal Guest
        {
            get
            {
                // Not authenticated
                return new Principal()
                {
                    Identity = new Identity()
                    {
                        IsAuthenticated = false,
                    }
                };
            }
        }

        public Principal()
        {
            InitializeMembers();
        }

        public Principal(Principal old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.identity = null;
            this.roles = new PrincipalRoleCollection();
        }

        private void CopyMembers(Principal old)
        {
            this.identity = new Identity(identity);
            this.roles = new PrincipalRoleCollection(old.roles);
        }

        public object Clone()
        {
            return new Principal(this);
        }

        #endregion

        public bool IsInRole(string role)
        {
            var idx = role.IndexOf('|');

            if (idx < 0)
            {
                throw Error.InvalidRole();
            }

            var g = role.Substring(0, idx);
            var r = role.Substring(idx + 1);

            return IsInRole(g, r);
        }

        public bool IsInRole(string group, string role)
        {
            // TODO: optimize this using hashtables

            foreach (var r in roles)
            {
                if (EntityAcl.Comparer.Compare(r.Group, group) == 0 &&
                    EntityAcl.Comparer.Compare(r.Role, role) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        #region Binary serialization

        public byte[] ToBinary()
        {
            using (var ms = new MemoryStream())
            {
                using (var w = new BinaryWriter(ms))
                {
                    ToBinary(w);
                }

                return ms.ToArray();
            }
        }

        public void ToBinary(BinaryWriter w)
        {
            w.Write('I');
            w.Write('D');
            w.Write((short)1);

            w.Write(Identity.IsAuthenticated);

            w.WriteNullString(Identity.Name);

            w.Write((short)roles.Count);

            foreach (var role in roles)
            {
                w.WriteNullString(role.Group);
                w.WriteNullString(role.Role);
            }
        }

        public static Principal FromBinary(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                using (var r = new BinaryReader(ms))
                {
                    return FromBinary(r);
                }
            }
        }

        public static Principal FromBinary(BinaryReader r)
        {
            var principal = new Principal();
            var identity = new Identity();
            principal.identity = identity;

            r.ReadChar();
            r.ReadChar();
            r.ReadInt16();

            identity.IsAuthenticated = r.ReadBoolean();
            identity.Name = r.ReadNullString();

            int rc = (int)r.ReadInt16();

            for (int i = 0; i < rc; i++)
            {
                var role = new PrincipalRole()
                {
                    Group = r.ReadNullString(),
                    Role = r.ReadNullString()
                };

                principal.roles.Add(role);
            }

            return principal;
        }

        #endregion
        #region XML serialization

        public string ToXml()
        {
            return Util.XmlConverter.ToXml(this, this.ToXml);
        }

        public void ToXml(Stream stream)
        {
            Util.XmlConverter.ToXml(this, stream, this.ToXml);
        }

        private void ToXml(XmlWriter w)
        {
            w.WriteStartElement("id");
            w.WriteAttributeString("name", identity.Name);
            w.WriteAttributeString("auth", identity.IsAuthenticated ? "1" : "0");

            PrincipalRole lastentry = null;

            foreach (var role in roles)
            {
                if (lastentry != null)
                {
                    w.WriteEndElement();
                }

                lastentry = role;

                w.WriteStartElement("group");
                w.WriteAttributeString("name", role.Group);
                w.WriteAttributeString("role", role.Role);
            }

            if (lastentry != null)
            {
                w.WriteEndElement();
            }

            w.WriteEndElement();
        }

        public static Principal FromXml(Stream stream)
        {
            return Util.XmlConverter.FromXml(stream, FromXml);
        }

        public static Principal FromXml(string xml)
        {
            return Util.XmlConverter.FromXml(xml, FromXml);
        }

        private static Principal FromXml(XmlReader r)
        {
            var principal = new Principal();
            var identity = new Identity();
            principal.identity = identity;

            r.Read();
            identity.Name = r.GetAttribute("name");
            identity.IsAuthenticated = r.GetAttribute("auth") == "1";
            r.ReadStartElement("id");

            // Read group and role membership
            while (r.NodeType == XmlNodeType.Element)
            {
                var role = new PrincipalRole();
                role.Group = r.GetAttribute("name");
                role.Role = r.GetAttribute("role");

                principal.roles.Add(role);

                r.Read();
            }

            if (r.NodeType == XmlNodeType.EndElement)
            {
                r.ReadEndElement();
            }

            return principal;
        }

        #endregion
    }
}
