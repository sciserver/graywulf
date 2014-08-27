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
        #region Static members to cache authenticators

        private static readonly object syncRoot = new object();
        private static bool initialized;

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
        }

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

            if (!initialized)
            {
                lock (syncRoot)
                {
                    LoadAuthenticators(domain);
                    initialized = true;
                }
            }

            return (AuthenticatorFactory)Activator.CreateInstance(
                type,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                null,
                null);
        }

        private static void LoadAuthenticators(Domain domain)
        {
            // Only load authenticators if settings are set
            if (domain.Settings.ContainsKey(Constants.SettingsAuthenticators))
            {
                allAuthenticators.Clear();
                webInteractiveAuthenticators.Clear();
                webRequestAuthenticators.Clear();
                restRequestAuthenticators.Clear();

                // Load authenticators from settings
                allAuthenticators.AddRange(
                    (Authenticator[])domain.Settings[Constants.SettingsAuthenticators].Value);

                // Initialize all authenticators
                foreach (var a in allAuthenticators)
                {
                    a.Initialize(domain);
                }

                // Sort authenticators by type
                webInteractiveAuthenticators.AddRange(
                    allAuthenticators.Where(i => (i.ProtocolType & AuthenticatorProtocolType.WebInteractive) != 0));

                webRequestAuthenticators.AddRange(
                    allAuthenticators.Where(i => (i.ProtocolType & AuthenticatorProtocolType.WebRequest) != 0));

                restRequestAuthenticators.AddRange(
                    allAuthenticators.Where(i => (i.ProtocolType & AuthenticatorProtocolType.RestRequest) != 0));
            }
        }

        #endregion
        #region Constructors and initializers

        protected AuthenticatorFactory()
        {
        }

        #endregion

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

    }
}
