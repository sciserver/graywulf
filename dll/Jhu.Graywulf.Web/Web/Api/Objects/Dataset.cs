using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Api
{
    [DataContract(Name="dataset")]
    public class Dataset
    {
        [DataMember(Name="name")]
        public string Name { get; set; }
    }
}
