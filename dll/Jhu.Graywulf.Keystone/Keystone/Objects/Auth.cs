using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Auth
    {
        [JsonProperty("identity")]
        public Identity Identity { get; set; }

        [JsonProperty("scope")]
        public Scope Scope { get; set; }
    }
}
