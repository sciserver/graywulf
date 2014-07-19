using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class Identity
    {
        [JsonProperty("methods")]
        public string[] Methods { get; set; }

        [JsonProperty("password")]
        public Password Password { get; set; }

        [JsonProperty("token")]
        public Token Token { get; set; }

        // TODO: does this work?
        //[JsonProperty("apikey")]
        //public string APIKey { get; set; }
    }
}
