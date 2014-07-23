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
        private static readonly object syncRoot = new object();
        private static bool initialized;
        
        private static List<Authenticator> allAuthenticators;
        private static List<Authenticator> webInteractiveAuthenticators;
        private static List<Authenticator> webRequestAuthenticators;
        private static List<Authenticator> restRequestAuthenticators;

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
            // Load authenticators from settings
            allAuthenticators = new List<Authenticator>(
                (Authenticator[])domain.Settings[Constants.SettingsAuthenticators].Value);

            // Initialize all authenticators
            foreach (var a in allAuthenticators)
            {
                a.Initialize(domain);
            }

            // Sort authenticators by type
            webInteractiveAuthenticators = new List<Authenticator>(
                allAuthenticators.Where(i => i.IsInteractive));

            webRequestAuthenticators = new List<Authenticator>(
                allAuthenticators.Where(i => !i.IsInteractive).ToArray());

            restRequestAuthenticators = new List<Authenticator>(
                allAuthenticators.Where(i => !i.IsInteractive).ToArray());
        }

        protected AuthenticatorFactory()
        {
        }

        public virtual IEnumerable<Authenticator> GetInteractiveAuthenticators()
        {
            return webInteractiveAuthenticators;
        }

        // TODO: delete this and add logic to somewhere else
        public Authenticator GetInteractiveAuthenticator(string protocol, string authority)
        {
            foreach (var a in GetInteractiveAuthenticators())
            {
                if (StringComparer.InvariantCultureIgnoreCase.Compare(a.ProtocolName, protocol) == 0 &&
                        StringComparer.InvariantCultureIgnoreCase.Compare(a.AuthorityUrl, authority) == 0)
                {
                    return a;
                }
            }

            return null;
        }

        public virtual IEnumerable<Authenticator> GetWebRequestAuthenticators()
        {
            return webRequestAuthenticators;
        }

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
