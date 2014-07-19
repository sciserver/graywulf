using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Endpoint : Entity
    {
        [JsonProperty("service_id")]
        public string ServiceID { get; set; }

        [JsonProperty("interface")]
        public string Interface { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("region_id")]
        public string RegionID { get; set; }
    }
}
