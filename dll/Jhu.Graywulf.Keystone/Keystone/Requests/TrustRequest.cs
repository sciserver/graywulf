using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Jhu.Graywulf.SimpleRestClient;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class TrustRequest
    {
        [JsonProperty("trust")]
        public Trust Trust { get; private set; }

        public static TrustRequest Create(Trust trust)
        {
            return new TrustRequest()
            {
                Trust = trust
            };
        }

        public static RestMessage<TrustRequest> CreateMessage(Trust trust)
        {
            return new RestMessage<TrustRequest>(Create(trust));
        }
    }
}
