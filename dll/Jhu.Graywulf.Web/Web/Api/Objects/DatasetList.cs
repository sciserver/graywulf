using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Web.Api
{
    [DataContract]
    public class DatasetList
    {
        [DataMember(Name="datasets")]
        public Dataset[] Dataset { get; set; }
    }
}
