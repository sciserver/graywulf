using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Jhu.Graywulf.SimpleRestClient;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class UserRequest
    {
        [JsonProperty("user")]
        public User User { get; private set; }

        public static UserRequest Create(User user)
        {
            return new UserRequest()
            {
                User = user
            };
        }

        public static RestMessage<UserRequest> CreateMessage(User user)
        {
            return new RestMessage<UserRequest>(Create(user));
        }
    }
}
