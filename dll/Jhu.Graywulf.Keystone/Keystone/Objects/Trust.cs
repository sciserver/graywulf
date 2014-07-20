using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Trust : Entity
    {
        private DateTime expiresAt;

        [JsonProperty("expires_at", ItemConverterType=typeof(IsoDateTimeConverter))]
        public DateTime ExpiresAt
        {
            get { return expiresAt; }
            set { expiresAt = value.ToUniversalTime(); }
        }

        [JsonProperty("project_id")]
        public string ProjectID { get; set; }

        [JsonProperty("roles")]
        public Role[] Roles { get; set; }

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
