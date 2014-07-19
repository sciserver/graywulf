using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Domain : Entity
    {
        public Domain Create(string name)
        {
            return new Domain()
            {
                Name = name
            };
        }
    }
}
