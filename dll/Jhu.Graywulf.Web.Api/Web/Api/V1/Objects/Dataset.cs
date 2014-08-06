using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract(Name = "dataset")]
    [Description("Represents a dataset.")]
    public class Dataset
    {
        [DataMember(Name = "name")]
        [Description("Name of the dataset.")]
        public string Name { get; set; }

        [DataMember(Name = "isMutable")]
        [Description("True, if the dataset can be modified by the user.")]
        public bool IsMutable { get; set; }

        public Dataset()
        {
        }

        public Dataset(Jhu.Graywulf.Schema.DatasetBase dataset)
        {
            this.Name = dataset.Name;
            this.IsMutable = dataset.IsMutable;
        }
    }
}
