using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Links
    {
        [JsonProperty("self")]
        public string Self { get; set; }

        [JsonProperty("previous")]
        public string Previous { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }
    }
}
