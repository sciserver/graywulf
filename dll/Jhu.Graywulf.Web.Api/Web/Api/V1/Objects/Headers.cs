using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [CollectionDataContract(Name="headers")]
    [Description("Represents a collection of headers used for user authentication.")]
    public class Headers : Dictionary<string, string>
    {
        public Headers(Dictionary<string, string> collection)
            :base(collection)
        {
        }
    }
}
