using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Api.V1.Objects
{
    [CollectionDataContract(Name="headers")]
    public class Headers : Dictionary<string, string>
    {
        public Headers(Dictionary<string, string> collection)
            :base(collection)
        {
        }
    }
}
