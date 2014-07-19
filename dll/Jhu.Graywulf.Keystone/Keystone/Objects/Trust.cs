using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Trust : Entity
    {
        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }

        [JsonProperty("project_id")]
        public string ProjectID { get; set; }

        [JsonProperty("trustor_user_id")]
        public string TrustorUserID { get; set; }

        [JsonProperty("trustor_user")]
        public User TrustorUser { get; set; }

        [JsonProperty("trustee_user_id")]
        public string TrusteeUserID { get; set; }

        [JsonProperty("trustee_user")]
        public User TrusteeUser { get; set; }

        [JsonProperty("impersonation")]
        public bool? Impersonation { get; set; }

        [JsonProperty("remaining_uses")]
        public int? RemainingUses { get; set; }

        // TODO: add roles
    }
}
