using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// Implements functionality to manage authenticator and
    /// protocols
    /// </summary>
    public class AuthenticatorFactory
    {
        private Domain domain;

        /// <summary>
        /// Creates an authenticator factory class from the
        /// type name.
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        public static AuthenticatorFactory Create(Domain domain)
        {
            Type type = null;

            if (!String.IsNullOrWhiteSpace(domain.AuthenticatorFactory))
            {
                type = Type.GetType(domain.AuthenticatorFactory);
            }

            // If config is incorrect, fall back to known types.
            if (type == null)
            {
                type = typeof(AuthenticatorFactory);
            }

            return (AuthenticatorFactory)Activator.CreateInstance(
                type,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new object[] { domain },
                null);
        }

        protected AuthenticatorFactory(Domain domain)
        {
            InitializeMembers();

            this.domain = domain;
        }

        private void InitializeMembers()
        {
            this.domain = null;
        }

        public IInteractiveAuthenticator[] CreateInteractiveAuthenticators()
        {
            var res = new List<IInteractiveAuthenticator>();

            if (domain.Settings.ContainsKey(Constants.SettingsOpenID))
            {
                res.AddRange((OpenIDAuthenticator[])domain.Settings[Constants.SettingsOpenID].Value);
            }

            return res.ToArray();
        }

        // TODO: delete this and add logic to somewhere else
        public IInteractiveAuthenticator CreateInteractiveAuthenticator(string protocol, string authority)
        {
            var q = from a in CreateInteractiveAuthenticators()
                    where
                        StringComparer.InvariantCultureIgnoreCase.Compare(a.Protocol, protocol) == 0 &&
                        StringComparer.InvariantCultureIgnoreCase.Compare(a.AuthorityUri, authority) == 0
                    select a;

            return q.FirstOrDefault();
        }

        public IRequestAuthenticator[] CreateWebRequestAuthenticators()
        {
            return new IRequestAuthenticator[0];
        }

        public IRequestAuthenticator[] CreateRestRequestAuthenticators()
        {
            return new IRequestAuthenticator[]
            {
                new FormsTicketAuthenticator()
            };
        }

    }
}
