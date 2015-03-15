using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Web.Api.V1
{
    [CollectionDataContract(Name="headers")]
    [Description("Represents a collection of headers used for user authentication.")]
    public class Headers : NameValueCollection
    {
        public Headers()
        {
        }

        public Headers(AuthenticationHeaderCollection collection)
        {
            foreach (var header in collection)
            {
                this.Add(header.Value.Name, header.Value.Value);
            }
        }
    }
}
