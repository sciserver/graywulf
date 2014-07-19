using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class UserResponse : ListResponse
    {
        [JsonProperty("user")]
        public User User { get; set; }
    }
}
