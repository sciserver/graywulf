using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Token
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("issued_at")]
        public DateTime IssuedAt { get; private set; }

        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; private set; }

        [JsonProperty("extras")]
        public object Extras { get; private set; }

        [JsonProperty("methods")]
        public string[] Methods { get; private set; }

        [JsonProperty("user")]
        public User User { get; private set; }

        [JsonProperty("project")]
        public Project Project { get; private set; }

        [JsonProperty("domain")]
        public Domain Domain { get; private set; }

        // TODO: add catalogs if necessary
        // https://github.com/openstack/identity-api/blob/master/v3/src/markdown/identity-api-v3.md#tokens

        // TODO: add bind if necessary

        [JsonProperty("OS-TRUST:trust")]
        public Trust Trust { get; set; }
    }
}
