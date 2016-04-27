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
        private HashSet<string> roles;

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

        public int RoleCount
        {
            get
            {
                return roles.Count;
            }
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

        public Principal(Identity identity)
        {
            InitializeMembers();

            this.identity = identity;
        }

        public Principal(Principal old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.identity = null;
            this.roles = new HashSet<string>(EntityAcl.Comparer);
        }

        private void CopyMembers(Principal old)
        {
            this.identity = new Identity(old.identity);
            this.roles = new HashSet<string>(old.roles);
        }

        public object Clone()
        {
            return new Principal(this);
        }

        #endregion

        public void AddRole(string role)
        {
            roles.Add(role);
        }

        public void AddRole(string group, string role)
        {
            roles.Add(group + "|" + role);
        }

        public bool IsInRole(string role)
        {
            return roles.Contains(role);
        }

        public bool IsInRole(string group, string role)
        {
            return roles.Contains(group + "|" + role);
        }

        public static bool IsInRole(IPrincipal principal, string group, string role)
        {
            return principal.IsInRole(group + "|" + role);
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
                w.WriteNullString(role);
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
                var role = r.ReadNullString();
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

            foreach (var role in roles)
            {
                w.WriteStartElement("role");
                w.WriteAttributeString("name", role);
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

            // Read roles
            while (r.NodeType == XmlNodeType.Element)
            {
                var role = r.GetAttribute("name");
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
