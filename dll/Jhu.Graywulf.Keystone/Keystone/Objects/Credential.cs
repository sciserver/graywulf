using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Credential
    {
        [JsonProperty("user_id")]
        public string DomainID { get; set; }

        [JsonProperty("project_id")]
        public string ProjectID { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("blob")]
        public byte[] Blob { get; set; }
    }
}
