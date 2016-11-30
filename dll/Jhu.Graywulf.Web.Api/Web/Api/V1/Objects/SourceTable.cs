using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a source table.")]
    public class SourceTable
    {
        private string dataset;
        private string table;

        [DataMember(Name = "dataset")]
        [Description("Source dataset.")]
        public string Dataset
        {
            get { return dataset; }
            set { dataset = value; }
        }

        [DataMember(Name = "table")]
        [Description("Name of source table.")]
        public string Table
        {
            get { return table; }
            set { table = value; }
        }
    }
}
