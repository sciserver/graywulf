using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

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

        public Dictionary<string, AccessType> EvaluateAccess(Identity identity)
        {
            var access = new Dictionary<string, AccessType>(EntityAcl.Comparer);

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
                    if (access.ContainsKey(ace.Access))
                    {
                        // Deny has precedence over grant
                        if (ace.Type == AccessType.Deny)
                        {
                            access[ace.Access] = AccessType.Deny;
                        }
                    }
                    else
                    {
                        access.Add(ace.Access, ace.Type);
                    }
                }
            }

            return access;
        }

        #region XML serialization

        public string ToXml()
        {
            var sw = new StringWriter();
            var w = new XmlTextWriter(sw);

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
                        throw new InvalidOperationException();
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

            return sw.ToString();
        }

        public static EntityAcl FromXml(string xml)
        {
            var sr = new StringReader(xml);
            var r = (XmlReader)XmlTextReader.Create(
                sr,
                new XmlReaderSettings()
                {
                    IgnoreWhitespace = true,
                });

            r.ReadStartElement("acl");

            var acl = new EntityAcl();

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

                r.ReadEndElement();
            }

            r.ReadEndElement();

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
