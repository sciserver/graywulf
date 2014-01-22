using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Security
{
    public class GraywulfIdentity : IIdentity
    {
        private string protocol;
        private string authority;
        private string identifier;
        private EntityProperty<User> userProperty;

        public string AuthenticationType
        {
            get { return Constants.AuthenticationTypeName; }
        }

        public string Protocol
        {
            get { return protocol; }
            set { protocol = value; }
        }

        public string Authority
        {
            get { return authority; }
            set { authority = value; }
        }

        public string Identifier
        {
            get { return identifier; }
            set { identifier = value; }
        }

        public bool IsAuthenticated
        {
            get { return !userProperty.IsEmpty; }
        }

        public EntityProperty<User> UserProperty
        {
            get { return userProperty; }
        }

        public string Name
        {
            get
            {
                return userProperty.IsEmpty ? null : userProperty.Value.Name;
            }
        }

        public User User
        {
            get { return userProperty.Value; }
            set { userProperty.Value = value; }
        }

        public GraywulfIdentity()
        {
            InitializeMembers(new StreamingContext());
        }

        public GraywulfIdentity(GraywulfIdentity old)
        {
            CopyMembers(old);
        }

        [OnSerializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.protocol = String.Empty;
            this.authority = String.Empty;
            this.identifier = String.Empty;
            this.userProperty = new EntityProperty<User>();
        }

        private void CopyMembers(GraywulfIdentity old)
        {
            this.protocol = old.protocol;
            this.authority = old.authority;
            this.identifier = old.identifier;
            this.userProperty = new EntityProperty<User>(old.userProperty);
        }

        public bool CompareByIdentifier(GraywulfIdentity other)
        {
            return
                StringComparer.InvariantCultureIgnoreCase.Compare(this.protocol, other.protocol) == 0 &&
                StringComparer.InvariantCultureIgnoreCase.Compare(this.authority, other.authority) == 0 &&
                StringComparer.InvariantCultureIgnoreCase.Compare(this.identifier, other.identifier) == 0;
        }

    }
}
