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
    public class AuthenticationFactory
    {
        #region Static members to cache authenticators

        /*private static readonly object syncRoot = new object();

        private static List<Authenticator> allAuthenticators;
        private static List<Authenticator> webInteractiveAuthenticators;
        private static List<Authenticator> webRequestAuthenticators;
        private static List<Authenticator> restRequestAuthenticators;

        static AuthenticatorFactory()
        {
            allAuthenticators = new List<Authenticator>();
            webInteractiveAuthenticators = new List<Authenticator>();
            webRequestAuthenticators = new List<Authenticator>();
            restRequestAuthenticators = new List<Authenticator>();
        }*/

        /// <summary>
        /// Creates an authenticator factory class from the
        /// type name.
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        public static AuthenticationFactory Create(Domain domain)
        {
            Type type = null;

            if (!String.IsNullOrWhiteSpace(domain.AuthenticatorFactory))
            {
                type = Type.GetType(domain.AuthenticatorFactory);
            }

            // If config is incorrect, fall back to known types.
            if (type == null)
            {
                type = typeof(AuthenticationFactory);
            }

            var factory = (AuthenticationFactory)Activator.CreateInstance(
                type,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                null,
                null);


            /*lock (syncRoot)
            {
                allAuthenticators = new List<Authenticator>(factory.CreateAuthenticators(domain));
            }*/

            return factory;
        }

        public virtual IEnumerable<Authentication> CreateAuthentications(Domain domain, AuthenticatorProtocolType protocolType)
        {
            var all = (Authentication[])domain.Settings[Constants.SettingsAuthenticators].Value;

            return all.Where(i => (i.ProtocolType & protocolType) != 0);
        }

        public virtual Authentication GetAuthentication(Domain domain, string protocol, string authority)
        {
            var all = (Authentication[])domain.Settings[Constants.SettingsAuthenticators].Value;

            foreach (var a in all)
            {
                if (StringComparer.InvariantCultureIgnoreCase.Compare(a.ProtocolName, protocol) == 0 &&
                        StringComparer.InvariantCultureIgnoreCase.Compare(a.AuthorityUri, authority) == 0)
                {
                    return a;
                }
            }

            return null;
        }

        #endregion
        #region Constructors and initializers

        protected AuthenticationFactory()
        {
        }

        #endregion

#if false
        /// <summary>
        /// Returns all authenticators requiring interactive authentication as
        /// an enumerable.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Authenticator> GetInteractiveAuthenticators()
        {
            return webInteractiveAuthenticators;
        }

        /// <summary>
        /// Returns an interactive authenticator (i.e. on requiring redirection to a web page)
        /// identified by the protocol name and authority url.
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="authority"></param>
        /// <returns></returns>
        public Authenticator GetInteractiveAuthenticator(string protocol, string authority)
        {
            foreach (var a in GetInteractiveAuthenticators())
            {
                if (StringComparer.InvariantCultureIgnoreCase.Compare(a.ProtocolName, protocol) == 0 &&
                        StringComparer.InvariantCultureIgnoreCase.Compare(a.AuthorityUri, authority) == 0)
                {
                    return a;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns all authenticators capable of authenticating HTTP web requests
        /// as an enumerable.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Authenticator> GetWebRequestAuthenticators()
        {
            return webRequestAuthenticators;
        }

        /// <summary>
        /// Returns all authenticators capable of authenticating REST requests
        /// as an enumerable.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Authenticator> GetRestRequestAuthenticators()
        {
            yield return new FormsTicketAuthenticator();

            foreach (var a in restRequestAuthenticators)
            {
                yield return a;
            }
        }
#endif
    }
}
