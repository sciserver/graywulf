using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Jhu.Graywulf.Entities.Util;

namespace Jhu.Graywulf.Entities.AccessControl
{
    public sealed class EntityAcl : IEnumerable<EntityAce>
    {
        public static readonly StringComparer Comparer = StringComparer.InvariantCultureIgnoreCase;

        #region Private member variables

        private bool isDirty;
        private string owner;
        private List<EntityAce> acl;

        #endregion
        #region Properties

        internal bool IsDirty
        {
            get { return isDirty; }
        }

        public string Owner
        {
            get { return owner; }
            set { owner = value; }
        }

        public int Count
        {
            get { return acl.Count; }
        }

        public EntityAce this[int index]
        {
            get { return acl[index]; }
        }

        #endregion
        #region Constructors and initializers

        public static EntityAcl Default
        {
            get
            {
                var acl = new EntityAcl();
                acl.Grant(DefaultIdentity.Owner, DefaultAccess.All);

                return acl;
            }
        }

        public EntityAcl()
        {
            InitializeMembers();
        }

        public EntityAcl(EntityAcl old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.isDirty = false;
            this.owner = null;
            this.acl = new List<EntityAce>();
        }

        private void CopyMembers(EntityAcl old)
        {
            this.isDirty = old.isDirty;
            this.owner = old.owner;
            this.acl = new List<EntityAce>(old.acl.Select(ace => (EntityAce)ace.Clone()));
        }

        #endregion

        private void SetDirty()
        {
            this.isDirty = true;
        }

        public void Grant(string name, string access)
        {
            acl.Add(new UserAce(name, access, AccessType.Grant));
            SetDirty();
        }

        public void Deny(string name, string access)
        {
            acl.Add(new UserAce(name, access, AccessType.Deny));
            SetDirty();
        }

        public void Grant(string name, string role, string access)
        {
            acl.Add(new GroupAce(name, role, access, AccessType.Grant));
            SetDirty();
        }

        public void Deny(string name, string role, string access)
        {
            acl.Add(new GroupAce(name, role, access, AccessType.Deny));
            SetDirty();
        }

        public void Clear(string name, string role, string access)
        {
            for (int i = acl.Count - 1; i >= 0; i--)
            {
                var ace = acl[i];

                if (ace is GroupAce &&
                    Comparer.Compare(ace.Access, access) == 0 &&
                    Comparer.Compare(ace.Name, name) == 0 &&
                    Comparer.Compare(((GroupAce)ace).Role, role) == 0)
                {
                    acl.RemoveAt(i);
                }
            }

            SetDirty();
        }

        public void Clear(string name, string access)
        {
            for (int i = acl.Count - 1; i >= 0; i--)
            {
                var ace = acl[i];

                if (Comparer.Compare(ace.Access, access) == 0 &&
                    Comparer.Compare(ace.Name, name) == 0)
                {
                    acl.RemoveAt(i);
                }
            }

            SetDirty();
        }

        public void Clear(string name)
        {
            for (int i = acl.Count - 1; i >= 0; i--)
            {
                var ace = acl[i];

                if (Comparer.Compare(ace.Name, name) == 0)
                {
                    acl.RemoveAt(i);
                }
            }

            SetDirty();
        }

        public void Clear()
        {
            acl.Clear();
            SetDirty();
        }

        public AccessCollection EvaluateAccess(Identity identity)
        {
            var access = new AccessCollection();

            bool isauthenticated = identity.IsAuthenticated;

            bool isowner =
                identity.IsAuthenticated &&
                owner != null &&
                EntityAcl.Comparer.Compare(identity.Name, owner) == 0;

            foreach (var ace in acl)
            {
                var applies = false;

                if (ace is UserAce)
                {
                    // Identity is owner of the resource
                    applies = applies ? true : (isowner && EntityAcl.Comparer.Compare(DefaultIdentity.Owner, ace.Name) == 0);

                    // Indentity has explicit rights
                    applies = applies ? true : (isauthenticated && EntityAcl.Comparer.Compare(identity.Name, ace.Name) == 0);

                    // Authenticated users
                    applies = applies ? true : (isauthenticated && EntityAcl.Comparer.Compare(DefaultIdentity.Public, ace.Name) == 0);

                    // Everyone is a guest
                    applies = applies ? true : (EntityAcl.Comparer.Compare(DefaultIdentity.Guest, ace.Name) == 0);
                }
                else if (ace is GroupAce)
                {
                    var gace = (GroupAce)ace;

                    // TODO: try to speed this up
                    foreach (var role in identity.Roles)
                    {
                        applies = applies ? true :
                            isauthenticated &&
                            EntityAcl.Comparer.Compare(role.Group, gace.Name) == 0 &&
                            EntityAcl.Comparer.Compare(role.Role, gace.Role) == 0;

                        if (applies)
                        {
                            break;
                        }
                    }
                }

                if (applies)
                {
                    access.Set(ace.Access, ace.Type);
                }
            }

            return access;
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
            w.Write('A');
            w.Write('C');
            w.Write('L');
            w.Write((short)1);

            w.WriteNullString(owner);

            EntityAce lastentry = null;

            foreach (var ace in acl)
            {
                if (ace.CompareTo(lastentry) != 0)
                {
                    if (ace is UserAce)
                    {
                        w.Write('U');
                        w.WriteNullString(ace.Name);
                    }
                    else if (ace is GroupAce)
                    {
                        w.Write('G');
                        w.WriteNullString(ace.Name);
                        w.WriteNullString(((GroupAce)ace).Role);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                w.Write('A');

                switch (ace.Type)
                {
                    case AccessType.Grant:
                        w.Write('+');
                        break;
                    case AccessType.Deny:
                        w.Write('-');
                        break;
                    default:
                        throw new NotImplementedException();
                }

                w.Write(ace.Access);
            }

            w.Write('X');
        }

        public static EntityAcl FromBinary(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                using (var r = new BinaryReader(ms))
                {
                    return FromBinary(r);
                }
            }
        }

        public static EntityAcl FromBinary(BinaryReader r)
        {
            var acl = new EntityAcl();

            r.ReadChar();
            r.ReadChar();
            r.ReadChar();
            r.ReadInt16();

            acl.owner = r.ReadNullString();

            EntityAce lastentry = null;
            
            var c = r.ReadChar();

            while (true)
            {
                switch (c)
                {
                    case 'X':
                        return acl;
                    case 'U':
                        lastentry = new UserAce()
                        {
                            Name = r.ReadNullString()
                        };
                        break;
                    case 'G':
                        lastentry = new GroupAce()
                        {
                            Name = r.ReadNullString(),
                            Role = r.ReadNullString()
                        };
                        break;
                    case 'A':
                        var nace = (EntityAce)lastentry.Clone();

                        switch (r.ReadChar())
                        {
                            case '+':
                                nace.Type = AccessType.Grant;
                                break;
                            case '-':
                                nace.Type = AccessType.Deny;
                                break;
                            default:
                                throw new InvalidOperationException();
                        }

                        nace.Access = r.ReadString();
                        acl.acl.Add(nace);
                        break;
                    default:
                        throw new InvalidOperationException();
                        
                }

                c = r.ReadChar();
            }
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
            w.WriteStartElement("acl");
            w.WriteAttributeString("owner", owner);

            EntityAce lastentry = null;

            foreach (var ace in acl)
            {
                if (ace.CompareTo(lastentry) != 0)
                {
                    if (lastentry != null)
                    {
                        w.WriteEndElement();
                    }

                    lastentry = ace;

                    if (ace is UserAce)
                    {
                        w.WriteStartElement("user");
                        w.WriteAttributeString("name", ((UserAce)ace).Name);
                    }
                    else if (ace is GroupAce)
                    {
                        w.WriteStartElement("group");
                        w.WriteAttributeString("name", ((GroupAce)ace).Name);
                        w.WriteAttributeString("role", ((GroupAce)ace).Role);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                var tag = ace.Type.ToString().ToLowerInvariant();
                w.WriteStartElement(tag);
                w.WriteAttributeString("access", ace.Access);
                w.WriteEndElement();
            }

            if (lastentry != null)
            {
                w.WriteEndElement();
            }

            w.WriteEndElement();
        }

        public static EntityAcl FromXml(Stream stream)
        {
            return Util.XmlConverter.FromXml(stream, FromXml);
        }

        public static EntityAcl FromXml(string xml)
        {
            return Util.XmlConverter.FromXml(xml, FromXml);
        }

        private static EntityAcl FromXml(XmlReader r)
        {
            r.Read();

            var acl = new EntityAcl();
            acl.owner = r.GetAttribute("owner");
            r.ReadStartElement("acl");

            // Read users and groups
            while (r.NodeType == XmlNodeType.Element)
            {
                EntityAce ace;

                switch (r.Name.ToLowerInvariant())
                {
                    case "user":
                        var user = new UserAce();
                        user.Name = r.GetAttribute("name");
                        ace = user;
                        break;
                    case "group":
                        var group = new GroupAce();
                        group.Name = r.GetAttribute("name");
                        group.Role = r.GetAttribute("role");
                        ace = group;
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                // Read access control entries

                r.Read();

                while (r.NodeType == XmlNodeType.Element)
                {
                    var nace = (EntityAce)ace.Clone();
                    nace.Access = r.GetAttribute("access");

                    switch (r.Name.ToLowerInvariant())
                    {
                        case "grant":
                            nace.Type = AccessType.Grant;
                            break;
                        case "deny":
                            nace.Type = AccessType.Deny;
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    acl.acl.Add(nace);

                    r.Read();
                }

                if (r.NodeType == XmlNodeType.EndElement)
                {
                    r.ReadEndElement();
                }
            }

            if (r.NodeType == XmlNodeType.EndElement)
            {
                r.ReadEndElement();
            }

            return acl;
        }

        #endregion
        #region Interface implementations

        public IEnumerator<EntityAce> GetEnumerator()
        {
            return acl.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return acl.GetEnumerator();
        }

        #endregion
    }
}
