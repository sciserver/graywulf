using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class UserListResponse : ListResponse
    {
        [JsonProperty("users")]
        public User[] Users { get; set; }
    }
}
