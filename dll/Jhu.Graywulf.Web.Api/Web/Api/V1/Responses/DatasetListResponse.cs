using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract(Name = "datasetList")]
    [Description("Represents a list of datasets.")]
    public class DatasetListResponse
    {
        [DataMember(Name = "datasets")]
        [Description("An array of datasets.")]
        public Dataset[] Datasets { get; set; }

        public DatasetListResponse()
        {
        }

        public DatasetListResponse(IEnumerable<Jhu.Graywulf.Schema.DatasetBase> datasets)
        {
            this.Datasets = datasets.Select(ds => new Dataset(ds)).ToArray();
        }
    }
}
