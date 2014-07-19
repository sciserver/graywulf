using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Version
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("updated")]
        public DateTime Updated { get; set; }
    }
}
