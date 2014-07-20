using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Scope
    {
        [JsonProperty("domain")]
        public Domain Domain { get; set; }

        [JsonProperty("project")]
        public Project Project { get; set; }

        [JsonProperty("OS-TRUST:trust")]
        public Trust Trust { get; set; }
    }
}
