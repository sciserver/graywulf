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
    public sealed class AccessControlList : IEnumerable<AccessControlEntry>
    {
        public static readonly StringComparer Comparer = StringComparer.InvariantCultureIgnoreCase;

        #region Private member variables

        private bool isDirty;
        private string owner;
        private Dictionary<string, AccessControlEntry> acl;

        #endregion
        #region Properties

        public bool IsDirty
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

        #endregion
        #region Constructors and initializers

        public static AccessControlList Default
        {
            get
            {
                var acl = new AccessControlList();
                acl.Grant(DefaultIdentity.Owner, DefaultAccess.All);

                return acl;
            }
        }

        public AccessControlList()
        {
            InitializeMembers();
        }

        public AccessControlList(AccessControlList old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.isDirty = false;
            this.owner = null;
            this.acl = new Dictionary<string, AccessControlEntry>(AccessControlList.Comparer);
        }

        private void CopyMembers(AccessControlList old)
        {
            this.isDirty = old.isDirty;
            this.owner = old.owner;
            this.acl = new Dictionary<string, AccessControlEntry>(AccessControlList.Comparer);

            foreach (var ace in acl.Values)
            {
                this.acl.Add(ace.UniqueKey, (AccessControlEntry)ace.Clone());
            }
        }

        #endregion

        private void SetDirty()
        {
            this.isDirty = true;
        }

        private void Add(AccessControlEntry ace)
        {
            var key = ace.UniqueKey;

            if (acl.ContainsKey(key))
            {
                acl[key].Access = ace.Access;
            }
            else
            {
                acl.Add(key, ace);
            }

            SetDirty();
        }

        private void Remove(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                acl.Remove(key);
            }
        }

        public void Grant(string name, string access)
        {
            Add(new UserAce(name, access, AccessType.Grant));
        }

        public void Deny(string name, string access)
        {
            Add(new UserAce(name, access, AccessType.Deny));
        }

        public void Grant(string name, string role, string access)
        {
            Add(new GroupAce(name, role, access, AccessType.Grant));
        }

        public void Deny(string name, string role, string access)
        {
            Add(new GroupAce(name, role, access, AccessType.Deny));
        }

        public void Clear(string name, string role, string access)
        {
            var keys = new List<string>();

            foreach (var key in acl.Keys)
            {
                var ace = acl[key];

                if (ace is GroupAce &&
                    Comparer.Compare(ace.Access, access) == 0 &&
                    Comparer.Compare(ace.Name, name) == 0 &&
                    Comparer.Compare(((GroupAce)ace).Role, role) == 0)
                {
                    keys.Add(key);
                }
            }

            Remove(keys);
            SetDirty();
        }

        public void Clear(string name, string access)
        {
            var keys = new List<string>();

            foreach (var key in acl.Keys)
            {
                var ace = acl[key];

                if (Comparer.Compare(ace.Access, access) == 0 &&
                    Comparer.Compare(ace.Name, name) == 0)
                {
                    keys.Add(key);
                }
            }

            Remove(keys);
            SetDirty();
        }

        public void Clear(string name)
        {
            var keys = new List<string>();

            foreach (var key in acl.Keys)
            {
                var ace = acl[key];

                if (Comparer.Compare(ace.Name, name) == 0)
                {
                    keys.Add(key);
                }
            }

            Remove(keys);
            SetDirty();
        }

        public void Clear()
        {
            acl.Clear();
            SetDirty();
        }

        public AccessCollection EvaluateAccess(IPrincipal principal)
        {
            var access = new AccessCollection();

            access.IsAuthenticated = principal.Identity.IsAuthenticated;

            access.IsOwner =
                principal.Identity.IsAuthenticated &&
                owner != null &&
                AccessControlList.Comparer.Compare(principal.Identity.Name, owner) == 0;

            foreach (var ace in acl.Values)
            {
                var applies = false;

                if (ace is UserAce)
                {
                    // Identity is owner of the resource
                    applies = applies ? true : (access.IsOwner && AccessControlList.Comparer.Compare(DefaultIdentity.Owner, ace.Name) == 0);

                    // Indentity has explicit rights
                    applies = applies ? true : (access.IsAuthenticated && AccessControlList.Comparer.Compare(principal.Identity.Name, ace.Name) == 0);

                    // Authenticated users
                    applies = applies ? true : (access.IsAuthenticated && AccessControlList.Comparer.Compare(DefaultIdentity.Public, ace.Name) == 0);

                    // Everyone is a guest
                    applies = applies ? true : (AccessControlList.Comparer.Compare(DefaultIdentity.Guest, ace.Name) == 0);
                }
                else if (ace is GroupAce)
                {
                    var gace = (GroupAce)ace;

                    applies = applies ? true :
                        access.IsAuthenticated &&
                        principal.IsInRole(gace.Name + "|" + gace.Role);
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

            AccessControlEntry lastentry = null;

            foreach (var ace in acl.Values)
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

        public static AccessControlList FromBinary(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            {
                using (var r = new BinaryReader(ms))
                {
                    return FromBinary(r);
                }
            }
        }

        public static AccessControlList FromBinary(BinaryReader r)
        {
            var acl = new AccessControlList();

            r.ReadChar();
            r.ReadChar();
            r.ReadChar();
            r.ReadInt16();

            acl.owner = r.ReadNullString();

            AccessControlEntry lastentry = null;
            
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
                        var nace = (AccessControlEntry)lastentry.Clone();

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
                        acl.acl.Add(nace.UniqueKey, nace);
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

            AccessControlEntry lastentry = null;

            foreach (var ace in acl.Values)
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

        public static AccessControlList FromXml(Stream stream)
        {
            return Util.XmlConverter.FromXml(stream, FromXml);
        }

        public static AccessControlList FromXml(string xml)
        {
            return Util.XmlConverter.FromXml(xml, FromXml);
        }

        private static AccessControlList FromXml(XmlReader r)
        {
            r.Read();

            var acl = new AccessControlList();
            acl.owner = r.GetAttribute("owner");
            r.ReadStartElement("acl");

            // Read users and groups
            while (r.NodeType == XmlNodeType.Element)
            {
                AccessControlEntry ace;

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
                    var nace = (AccessControlEntry)ace.Clone();
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

                    acl.acl.Add(nace.UniqueKey, nace);

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

        public IEnumerator<AccessControlEntry> GetEnumerator()
        {
            return acl.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return acl.Values.GetEnumerator();
        }

        #endregion
    }
}
