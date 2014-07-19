using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Policy : Entity
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("blob")]
        public byte[] Blob { get; set; }
    }
}
