using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Web.Api
{
    [DataContract(Name = "datasetList")]
    public class DatasetList
    {
        [DataMember(Name = "datasets")]
        public Dataset[] Datasets { get; set; }
    }
}
