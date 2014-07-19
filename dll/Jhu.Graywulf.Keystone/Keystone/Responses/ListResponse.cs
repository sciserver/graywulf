using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class ListResponse
    {
        [JsonProperty("links")]
        public Links Links { get; set; }

        [JsonProperty("truncated")]
        public bool Truncated { get; set; }
    }
}
