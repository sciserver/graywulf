using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    public class User : Entity
    {
        [JsonProperty("domain_id")]
        public string DomainID { get; set; }

        [JsonProperty("default_project_id")]
        public string DefaultProjectID { get; set; }

        [JsonProperty("domain")]
        public Domain Domain { get; set; }

        [JsonProperty("email")]
        public Domain Email { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("original_password")]
        internal string OriginalPassword { get; set; }
    }
}
