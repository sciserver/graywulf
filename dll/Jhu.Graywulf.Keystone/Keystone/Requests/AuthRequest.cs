using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Jhu.Graywulf.SimpleRestClient;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class AuthRequest
    {
        [JsonProperty("auth")]
        public Auth Auth { get; private set; }

        public static AuthRequest Create(string domain, string username, string password, Domain scopeDomain, Project scopeProject)
        {
            Scope scope = null;

            if (scopeDomain != null || scopeProject != null)
            {
                scope = new Scope()
                {
                    Domain = scopeDomain,
                    Project = scopeProject,
                };
            }

            return new AuthRequest()
            {
                Auth = new Auth()
                {
                    Identity = new Identity()
                    {
                        Methods = new[] { "password" },
                        Password = new Password()
                        {
                            User = new User()
                            {
                                Domain = new Domain()
                                {
                                    Name = domain
                                },
                                Name = username,
                                Password = password
                            }
                        },
                    },
                    Scope = scope
                }
            };
        }

        public static AuthRequest Create(Token token)
        {
            return new AuthRequest()
            {
                Auth = new Auth()
                {
                    Identity = new Identity()
                    {
                        Methods = new[] { "token" },
                        Token = token
                    }
                }
            };
        }

        public static RestMessage<AuthRequest> CreateMessage(string domain, string username, string password, Domain scopeDomain, Project scopeProject)
        {
            return new RestMessage<AuthRequest>(Create(domain, username, password, scopeDomain, scopeProject));
        }

        public static RestMessage<AuthRequest> CreateMessage(Token token)
        {
            return new RestMessage<AuthRequest>(Create(token));
        }
    }
}
