using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Jhu.Graywulf.SimpleRestClient;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class DomainRequest
    {
        [JsonProperty("domain")]
        public Domain Domain { get; private set; }

        public static DomainRequest Create(Domain domain)
        {
            return new DomainRequest()
            {
                Domain = domain
            };
        }

        public static RestMessage<DomainRequest> CreateMessage(Domain domain)
        {
            return new RestMessage<DomainRequest>(Create(domain));
        }
    }
}
