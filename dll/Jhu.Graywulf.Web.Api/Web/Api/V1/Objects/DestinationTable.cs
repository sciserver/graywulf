using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a destination table.")]
    public class DestinationTable
    {
        private string dataset;
        private string table;

        [DataMember(Name = "dataset")]
        [Description("Destination dataset.")]
        public string Dataset
        {
            get { return dataset; }
            set { dataset = value; }
        }

        [DataMember(Name = "table")]
        [Description("Name of destination table.")]
        public string Table
        {
            get { return table; }
            set { table = value; }
        }
    }
}
