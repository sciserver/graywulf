using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Security
{
    public class AuthenticatorFactory
    {

        public static AuthenticatorFactory Create(string typename)
        {
            Type type = null;

            if (typename != null)
            {
                type = Type.GetType(typename);
            }

            // If config is incorrect, fall back to known types.
            if (type == null)
            {
                type = typeof(AuthenticatorFactory);
            }

            return (AuthenticatorFactory)Activator.CreateInstance(type, true);
        }

        public AuthenticatorBase[] GetAuthenticators()
        {
            return new[] 
            {
                new OpenIDAuthenticator()
                {
                    Authority="https://sso.usvao.org/openid/provider",
                    DisplayName = "VO OpenID",
                    DiscoveryUrl = "https://sso.usvao.org/openid/provider_id"
                    //DiscoveryUrl = "https://www.google.com/accounts/o8/id"
                }
            };
        }

        public AuthenticatorBase GetAuthenticator(string protocol, string authority)
        {
            var q = from a in GetAuthenticators()
                    where
                        StringComparer.InvariantCultureIgnoreCase.Compare(a.Protocol, protocol) == 0 &&
                        StringComparer.InvariantCultureIgnoreCase.Compare(a.Authority, authority) == 0
                    select a;

            return q.FirstOrDefault();
        }
    }
}
