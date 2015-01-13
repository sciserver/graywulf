using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Security;
using System.Configuration;
using Jhu.Graywulf.Check;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Security
{
    /// <summary>
    /// Implements functionality to manage authenticator and
    /// protocols
    /// </summary>
    public class AuthenticationFactory : ICheckable
    {
        #region Constructors and initializers

        protected AuthenticationFactory()
        {
        }

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

            return factory;
        }

        #endregion

        protected virtual IEnumerable<Authentication> CreateAuthentications()
        {
            if (FormsAuthentication.IsEnabled)
            {
                yield return new FormsTicketAuthentication();
            }
        }

        public IEnumerable<Authentication> GetAuthentications(AuthenticatorProtocolType protocolType)
        {
            return CreateAuthentications().Where(i => (i.ProtocolType & protocolType) != 0);
        }

        public Authentication GetAuthentication(string protocol, string authority)
        {
            return CreateAuthentications().Where(i =>
                StringComparer.InvariantCultureIgnoreCase.Compare(i.ProtocolName, protocol) == 0 &&
                StringComparer.InvariantCultureIgnoreCase.Compare(i.AuthorityUri, authority) == 0).FirstOrDefault();
        }

        #region Check routines

        public virtual IEnumerable<CheckRoutineBase> GetCheckRoutines()
        {
            foreach (var auth in CreateAuthentications())
            {
                foreach (var c in auth.GetCheckRoutines())
                {
                    yield return c;
                }
            }
        }

        #endregion
    }
}
