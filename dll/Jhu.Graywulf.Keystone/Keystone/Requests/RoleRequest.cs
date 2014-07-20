using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Jhu.Graywulf.SimpleRestClient;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class RoleRequest
    {
        [JsonProperty("role")]
        public Role Role { get; private set; }

        public static RoleRequest Create(Role role)
        {
            return new RoleRequest()
            {
                Role = role
            };
        }

        public static RestMessage<RoleRequest> CreateMessage(Role role)
        {
            return new RestMessage<RoleRequest>(Create(role));
        }
    }
}
