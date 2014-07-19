using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Jhu.Graywulf.Keystone.Objects
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Region
    {
        [JsonProperty("parent_region_id")]
        public string ParentRegionID { get; set; }
    }
}
