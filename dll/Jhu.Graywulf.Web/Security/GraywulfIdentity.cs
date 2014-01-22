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
        private bool isAuthenticated;
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
            get { return isAuthenticated; }
            internal set { isAuthenticated = value; }
        }

        public EntityProperty<User> UserProperty
        {
            get { return userProperty; }
        }

        public string Name
        {
            get
            {
                if (userProperty.IsEmpty)
                {
                    return null;
                }
                else
                {
                    // This should always work
                    var idx = userProperty.Name.LastIndexOf('.');
                    return userProperty.Name.Substring(idx + 1);
                }
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


        /// <summary>
        /// Loads the user from the graywulf registry
        /// </summary>
        /// <param name="identity"></param>
        public void LoadUser(Domain domain)
        {
            using (var registryContext = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.AutoCommit))
            {
                var uf = new UserFactory(registryContext);

                try
                {
                    switch (protocol)
                    {
                        case Constants.ProtocolNameForms:
                            userProperty.Value = uf.LoadUser(identifier);
                            break;
                        case Constants.ProtocolNameWindows:
                            // TODO: implement NTLM auth
                            throw new NotImplementedException();
                        default:
                            // All other cases use lookup by protocol name
                            userProperty.Value = uf.FindUserByIdentity(domain, protocol, authority, identifier);
                            break;
                    }

                    IsAuthenticated = true;
                }
                catch (EntityNotFoundException)
                {
                    isAuthenticated = false;
                }
            }
        }
    }
}
