using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.SimpleRestClient
{
    public class RestHeaderCollection : Dictionary<string, RestHeader>
    {
        public RestHeaderCollection()
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
        }

        public void Add(RestHeader item)
        {
            Add(item.Name, item);
        }

        public void Set(RestHeader item)
        {
            this[item.Name] = item;
        }
    }
}
